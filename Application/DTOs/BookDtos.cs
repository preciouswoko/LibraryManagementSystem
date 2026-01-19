using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{

    /// <summary>
    /// DTO for creating a new book.
    /// Separates external API contract from internal domain model.
    /// </summary>
    public class CreateBookDto
    {
        public required string Title { get; set; } = string.Empty;
        public required string Author { get; set; } = string.Empty;
        public required string ISBN { get; set; } = string.Empty;
        public required DateTime PublishedDate { get; set; }
    }

    /// <summary>
    /// DTO for updating an existing book.
    /// </summary>
    public class UpdateBookDto
    {
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public DateTime PublishedDate { get; set; }
    }

    /// <summary>
    /// DTO for book responses.
    /// Includes all book information for API consumers.
    /// </summary>
    public class BookDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public DateTime PublishedDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// DTO for paginated book results.
    /// </summary>
    public class PagedBooksDto
    {
        public IEnumerable<BookDto> Items { get; set; } = new List<BookDto>();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }
}
