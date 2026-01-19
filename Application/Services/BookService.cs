using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class BookService : IBookService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;
        private readonly ILogger<BookService> _logger;
        private const string BooksCacheKey = "books_all";
        private const string BookCacheKeyPrefix = "book_";
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(5);

        public BookService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMemoryCache cache,
            ILogger<BookService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<BookDto> CreateBookAsync(CreateBookDto createBookDto, CancellationToken cancellationToken = default)
        {
            if (createBookDto == null)
            {
                _logger.LogError("CreateBookAsync called with null createBookDto");
                throw new ArgumentNullException(nameof(createBookDto));
            }

            _logger.LogInformation("Creating book: {Title} by {Author}", createBookDto.Title, createBookDto.Author);

            // Check ISBN uniqueness
            var existingBook = await _unitOfWork.Books.FindAsync(
                b => b.ISBN == createBookDto.ISBN,
                cancellationToken);

            if (existingBook.Any())
            {
                _logger.LogWarning("Duplicate ISBN detected: {ISBN}", createBookDto.ISBN);
                throw new InvalidOperationException($"A book with ISBN '{createBookDto.ISBN}' already exists.");
            }

            var book = _mapper.Map<Book>(createBookDto);
            book.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                await _unitOfWork.Books.AddAsync(book, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("Book created successfully - ID: {Id}", book.Id);
                InvalidateCache();

                return _mapper.Map<BookDto>(book);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogError("Failed to create book: {Title}", createBookDto.Title);
                throw;
            }
        }

        public async Task<BookDto?> GetBookByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var cacheKey = $"{BookCacheKeyPrefix}{id}";

            // Try cache first
            if (_cache.TryGetValue(cacheKey, out BookDto? cachedBook))
            {
                return cachedBook;
            }

            var book = await _unitOfWork.Books.GetByIdAsync(id, cancellationToken);

            if (book == null)
            {
                return null;
            }

            var bookDto = _mapper.Map<BookDto>(book);

            // Cache the result
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(_cacheExpiration)
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(30));

            _cache.Set(cacheKey, bookDto, cacheOptions);

            return bookDto;
        }

        public async Task<PagedBooksDto> GetBooksAsync(
            int? pageNumber = null,
            int? pageSize = null,
            string? search = null,
            CancellationToken cancellationToken = default)
        {
            // Validate pagination
            if (pageNumber.HasValue && pageNumber.Value < 1)
                throw new ArgumentException("Page number must be greater than 0", nameof(pageNumber));

            if (pageSize.HasValue && pageSize.Value < 1)
                throw new ArgumentException("Page size must be greater than 0", nameof(pageSize));

            // Handle paginated requests
            if (pageNumber.HasValue && pageSize.HasValue)
            {
                var (items, totalCount) = await GetPaginatedBooksAsync(pageNumber.Value, pageSize.Value, search, cancellationToken);

                return new PagedBooksDto
                {
                    Items = _mapper.Map<IEnumerable<BookDto>>(items),
                    PageNumber = pageNumber.Value,
                    PageSize = pageSize.Value,
                    TotalCount = totalCount
                };
            }

            // Handle non-paginated requests with caching
            return await GetAllBooksCachedAsync(search, cancellationToken);
        }

        public async Task<BookDto> UpdateBookAsync(int id, UpdateBookDto updateBookDto, CancellationToken cancellationToken = default)
        {
            if (updateBookDto == null)
                throw new ArgumentNullException(nameof(updateBookDto));

            var book = await _unitOfWork.Books.GetByIdAsync(id, cancellationToken);

            if (book == null)
                throw new KeyNotFoundException($"Book with ID {id} not found.");

            // Check ISBN uniqueness (excluding current book)
            var existingBook = await _unitOfWork.Books.FindAsync(
                b => b.ISBN == updateBookDto.ISBN && b.Id != id,
                cancellationToken);

            if (existingBook.Any())
                throw new InvalidOperationException($"Another book with ISBN '{updateBookDto.ISBN}' already exists.");

            // Update book properties
            book.Title = updateBookDto.Title;
            book.Author = updateBookDto.Author;
            book.ISBN = updateBookDto.ISBN;
            book.PublishedDate = updateBookDto.PublishedDate;
            book.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                _unitOfWork.Books.Update(book);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("Book {Id} updated successfully", id);
                InvalidateCache();
                InvalidateBookCache(id);

                return _mapper.Map<BookDto>(book);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogError("Failed to update book {Id}", id);
                throw;
            }
        }

        public async Task<bool> DeleteBookAsync(int id, CancellationToken cancellationToken = default)
        {
            var book = await _unitOfWork.Books.GetByIdAsync(id, cancellationToken);

            if (book == null)
                return false;

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                _unitOfWork.Books.Delete(book);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("Book {Id} deleted successfully", id);
                InvalidateCache();
                InvalidateBookCache(id);

                return true;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogError("Failed to delete book {Id}", id);
                throw;
            }
        }

        private async Task<(IEnumerable<Book> items, int totalCount)> GetPaginatedBooksAsync(
            int pageNumber, int pageSize, string? search, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchLower = search.ToLower();
                var pagedResult = await _unitOfWork.Books.GetPagedAsync(
                    pageNumber,
                    pageSize,
                    b => b.Title.ToLower().Contains(searchLower) ||
                         b.Author.ToLower().Contains(searchLower),
                    cancellationToken);

                return (pagedResult.Items, pagedResult.TotalCount);
            }
            else
            {
                var pagedResult = await _unitOfWork.Books.GetPagedAsync(
                    pageNumber,
                    pageSize,
                    null,
                    cancellationToken);

                return (pagedResult.Items, pagedResult.TotalCount);
            }
        }

        private async Task<PagedBooksDto> GetAllBooksCachedAsync(string? search, CancellationToken cancellationToken)
        {
            var cacheKey = string.IsNullOrWhiteSpace(search) ? BooksCacheKey : $"{BooksCacheKey}_{search.ToLower()}";

            if (_cache.TryGetValue(cacheKey, out IEnumerable<BookDto>? cachedBooks) && cachedBooks != null)
            {
                return new PagedBooksDto
                {
                    Items = cachedBooks,
                    PageNumber = 1,
                    PageSize = cachedBooks.Count(),
                    TotalCount = cachedBooks.Count()
                };
            }

            IEnumerable<Book> books;
            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchLower = search.ToLower();
                books = await _unitOfWork.Books.FindAsync(
                    b => b.Title.ToLower().Contains(searchLower) ||
                         b.Author.ToLower().Contains(searchLower),
                    cancellationToken);
            }
            else
            {
                books = await _unitOfWork.Books.GetAllAsync(cancellationToken);
            }

            var bookDtos = _mapper.Map<IEnumerable<BookDto>>(books).ToList();

            // Cache the results
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(_cacheExpiration)
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(30));

            _cache.Set(cacheKey, bookDtos, cacheOptions);

            return new PagedBooksDto
            {
                Items = bookDtos,
                PageNumber = 1,
                PageSize = bookDtos.Count,
                TotalCount = bookDtos.Count
            };
        }

        private void InvalidateCache()
        {
            _cache.Remove(BooksCacheKey);
        }

        private void InvalidateBookCache(int bookId)
        {
            var cacheKey = $"{BookCacheKeyPrefix}{bookId}";
            _cache.Remove(cacheKey);
        }
    }
}