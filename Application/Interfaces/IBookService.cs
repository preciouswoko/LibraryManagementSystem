using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IBookService
    {
        Task<BookDto> CreateBookAsync(CreateBookDto createBookDto, CancellationToken cancellationToken = default);
        Task<bool> DeleteBookAsync(int id, CancellationToken cancellationToken = default);
        Task<BookDto?> GetBookByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<PagedBooksDto> GetBooksAsync(int? pageNumber = null, int? pageSize = null, string? search = null, CancellationToken cancellationToken = default);
        Task<BookDto> UpdateBookAsync(int id, UpdateBookDto updateBookDto, CancellationToken cancellationToken = default);
    }
}
