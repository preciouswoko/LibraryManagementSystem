using API.Middleware;
using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;
        private readonly ILogger<BooksController> _logger;

        public BooksController(IBookService bookService, ILogger<BooksController> logger)
        {
            _bookService = bookService ?? throw new ArgumentNullException(nameof(bookService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost]
        [ProducesResponseType(typeof(BookDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<BookDto>> CreateBook(
            [FromBody] CreateBookDto createBookDto,
            CancellationToken cancellationToken)
        {
            try
            {
                var book = await _bookService.CreateBookAsync(createBookDto, cancellationToken);
                return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
            }
            catch (Exception ex) when (ex is ArgumentNullException or InvalidOperationException)
            {
                return BadRequest(CreateErrorResponse(400, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating book");
                return StatusCode(500, CreateErrorResponse(500, "An unexpected error occurred while creating the book."));
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedBooksDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<PagedBooksDto>> GetBooks(
            [FromQuery] int? pageNumber,
            [FromQuery] int? pageSize,
            [FromQuery] string? search,
            CancellationToken cancellationToken)
        {
            try
            {
                var books = await _bookService.GetBooksAsync(pageNumber, pageSize, search, cancellationToken);
                return Ok(books);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(CreateErrorResponse(400, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving books");
                return StatusCode(500, CreateErrorResponse(500, "An unexpected error occurred while retrieving books."));
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BookDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<BookDto>> GetBook(int id, CancellationToken cancellationToken)
        {
            try
            {
                var book = await _bookService.GetBookByIdAsync(id, cancellationToken);

                if (book == null)
                {
                    return NotFound(CreateErrorResponse(404, $"Book with ID {id} not found."));
                }

                return Ok(book);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving book {Id}", id);
                return StatusCode(500, CreateErrorResponse(500, "An unexpected error occurred while retrieving the book."));
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(BookDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<BookDto>> UpdateBook(
            int id,
            [FromBody] UpdateBookDto updateBookDto,
            CancellationToken cancellationToken)
        {
            try
            {
                var book = await _bookService.UpdateBookAsync(id, updateBookDto, cancellationToken);
                return Ok(book);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(CreateErrorResponse(400, ex.Message));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(CreateErrorResponse(404, ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(CreateErrorResponse(400, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating book {Id}", id);
                return StatusCode(500, CreateErrorResponse(500, "An unexpected error occurred while updating the book."));
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteBook(int id, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _bookService.DeleteBookAsync(id, cancellationToken);

                if (!result)
                {
                    return NotFound(CreateErrorResponse(404, $"Book with ID {id} not found."));
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting book {Id}", id);
                return StatusCode(500, CreateErrorResponse(500, "An unexpected error occurred while deleting the book."));
            }
        }

        private ErrorResponse CreateErrorResponse(int statusCode, string message)
        {
            return new ErrorResponse
            {
                StatusCode = statusCode,
                Message = message,
                Path = HttpContext.Request.Path,
                Timestamp = DateTime.UtcNow
            };
        }
    }
}