using API.Extensions;
using API.Middleware;
using Infrastructure.Data;
using API.Extensions;
using API.Middleware;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Configure database
builder.Services.AddDatabaseContext(builder.Configuration);

// Register application services
builder.Services.AddApplicationServices();

// Configure JWT authentication
builder.Services.AddJwtAuthentication(builder.Configuration);

// Configure Swagger documentation
builder.Services.AddSwaggerDocumentation();

// Configure CORS
builder.Services.AddCorsPolicy();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Library Management System API v1");
        options.RoutePrefix = "swagger";
        options.DocumentTitle = "Library Management System API";
        options.DisplayRequestDuration();
    });
}

// Global exception handling middleware (MUST be first)
app.UseMiddleware<ExceptionHandlingMiddleware>();

// HTTPS redirection
app.UseHttpsRedirection();

// CORS (before authentication)
app.UseCors("AllowAll");

// Authentication & Authorization (in this order!)
app.UseAuthentication();
app.UseAuthorization();

// Map controllers
app.MapControllers();

// Apply database migrations and seed data on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var logger = services.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("Applying database migrations...");

        // Apply pending migrations
        context.Database.Migrate();

        logger.LogInformation("Database migration completed successfully.");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database: {Message}", ex.Message);

        // Don't throw in production - allow app to start even if migration fails
        if (app.Environment.IsDevelopment())
        {
            throw;
        }
    }
}

// Log startup information
app.Logger.LogInformation("Application started successfully");
app.Logger.LogInformation("Environment: {Environment}", app.Environment.EnvironmentName);
app.Logger.LogInformation("Swagger UI available at: /swagger");

app.Run();

