using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
   

    /// <summary>
    /// Represents a book in the library system.
    /// Follows the Single Responsibility Principle - manages only book-related data.
    /// </summary>
    public class Book
    {
        /// <summary>
        /// Unique identifier for the book.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Title of the book.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Author of the book.
        /// </summary>
        public string Author { get; set; } = string.Empty;

        /// <summary>
        /// International Standard Book Number.
        /// Must be unique across all books.
        /// </summary>
        public string ISBN { get; set; } = string.Empty;

        /// <summary>
        /// Date when the book was published.
        /// </summary>
        public DateTime PublishedDate { get; set; }

        /// <summary>
        /// Timestamp when the book record was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Timestamp when the book record was last updated.
        /// Null if never updated.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
    }
}
