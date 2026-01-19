using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;

namespace API.Middleware
{
 


    /// <summary>
    /// Global exception handling middleware.
    /// Centralizes error handling and provides consistent error responses.
    /// Follows the Single Responsibility Principle.
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IHostEnvironment _environment;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger,
            IHostEnvironment environment)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        /// <summary>
        /// Handles exceptions and returns appropriate HTTP responses.
        /// Maps exception types to HTTP status codes.
        /// </summary>
        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var requestId = context.TraceIdentifier;
            _logger.LogError(exception,
                "Request {RequestId}: An error occurred: {Message}",
                requestId, exception.Message);
         //   _logger.LogError(exception, "An error occurred: {Message}", exception.Message);

            var response = context.Response;
            response.ContentType = "application/json";

            var errorResponse = new ErrorResponse
            {
                Message = exception.Message,
                Timestamp = DateTime.UtcNow,
                Path = context.Request.Path
            };

            // Map exception types to HTTP status codes
            response.StatusCode = exception switch
            {
                KeyNotFoundException => (int)HttpStatusCode.NotFound,
                UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
                InvalidOperationException => (int)HttpStatusCode.BadRequest,
                ArgumentException => (int)HttpStatusCode.BadRequest,
               // ArgumentNullException => (int)HttpStatusCode.BadRequest,
                ValidationException => (int)HttpStatusCode.BadRequest, // For FluentValidation
                NotImplementedException => (int)HttpStatusCode.NotImplemented,
                TimeoutException => (int)HttpStatusCode.GatewayTimeout,
                DbUpdateException => (int)HttpStatusCode.Conflict, // For EF Core conflicts
                _ => (int)HttpStatusCode.InternalServerError
            };

        
            errorResponse.StatusCode = response.StatusCode;
            errorResponse.CorrelationId = context.TraceIdentifier;

            // Don't expose internal error details in production
            if (response.StatusCode == (int)HttpStatusCode.InternalServerError && !_environment.IsDevelopment())
            {
                errorResponse.Message = "An internal server error occurred. Please try again later.";
                errorResponse.Details = null;
            }
            else if (_environment.IsDevelopment())
            {
                // Include stack trace in development
                errorResponse.Details = exception.StackTrace;
            }

            var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = _environment.IsDevelopment()
            });

            await response.WriteAsync(jsonResponse);
        }
    }

    /// <summary>
    /// Standard error response model.
    /// </summary>
    public class ErrorResponse
    {
        /// <summary>
        /// HTTP status code
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Error message
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Request path where error occurred
        /// </summary>
        public string Path { get; set; } = string.Empty;

        /// <summary>
        /// Timestamp when error occurred (UTC)
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Additional details (only in development)
        /// </summary>
        public string? Details { get; set; }
        public string CorrelationId { get; set; } = string.Empty; // For distributed tracing
    }
}
