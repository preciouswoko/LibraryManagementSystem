# Library Management System API

ASP.NET Core 8.0 RESTful API with Clean Architecture, SOLID principles, JWT authentication, and ACID compliance.

## Features

- ✅ Complete CRUD operations for books
- ✅ JWT-based authentication
- ✅ Clean Architecture (4 layers)
- ✅ SOLID principles
- ✅ Entity Framework Core + SQL Server
- ✅ Memory caching
- ✅ Search & Pagination
- ✅ Swagger/OpenAPI documentation
- ✅ Database seeding
- ✅ ACID transaction management

## Quick Start

### Prerequisites
- .NET 8.0 SDK
- SQL Server 2019+ or Express

### Setup

1. **Update Connection String** in `src/API/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=LibraryManagementDb;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

For SQL Server Auth:
```json
"DefaultConnection": "Server=localhost;Database=LibraryManagementDb;User Id=sa;Password=YourPass;TrustServerCertificate=true;"
```

2. **Restore & Run**:
```bash
dotnet restore
dotnet run --project src/API/LibraryManagementSystem.API.csproj
```

3. **Open Swagger**: https://localhost:7001/swagger

## API Endpoints

### Authentication
```
POST /api/auth/register - Register new user
POST /api/auth/login    - Login and get JWT token
```

### Books (Protected - Requires JWT)
```
POST   /api/books           - Create book
GET    /api/books           - Get all books
GET    /api/books?search=x  - Search books
GET    /api/books?pageNumber=1&pageSize=10 - Paginated books
GET    /api/books/{id}      - Get book by ID
PUT    /api/books/{id}      - Update book
DELETE /api/books/{id}      - Delete book
```

## Testing

### Using Swagger UI
1. Go to https://localhost:7001/swagger
2. Login with seeded admin:
   ```json
   {
     "email": "admin@library.com",
     "password": "Admin123!"
   }
   ```
3. Copy the token
4. Click "Authorize" button
5. Enter: `Bearer {your_token}`
6. Test endpoints

### Using cURL
```bash
# Login
curl -X POST https://localhost:7001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@library.com","password":"Admin123!"}' -k

# Get Books (replace {TOKEN})
curl -X GET https://localhost:7001/api/books \
  -H "Authorization: Bearer {TOKEN}" -k

# Create Book
curl -X POST https://localhost:7001/api/books \
  -H "Authorization: Bearer {TOKEN}" \
  -H "Content-Type: application/json" \
  -d '{"title":"The Hobbit","author":"J.R.R. Tolkien","isbn":"978-0547928227","publishedDate":"1937-09-21"}' -k
```

## Architecture

```
LibraryManagementSystem/
├── src/
│   ├── Domain/         # Entities, Interfaces (no dependencies)
│   ├── Application/    # Business logic, DTOs, Services
│   ├── Infrastructure/ # Data access, EF Core, Repositories
│   └── API/           # Controllers, Middleware, JWT
```

### SOLID Principles
- **S**ingle Responsibility - Each class has one purpose
- **O**pen/Closed - Generic repository pattern
- **L**iskov Substitution - Interface-based design
- **I**nterface Segregation - Focused interfaces
- **D**ependency Inversion - DI for all dependencies

### ACID Compliance
- **Atomicity** - Unit of Work pattern with transactions
- **Consistency** - Database constraints + validations
- **Isolation** - EF Core Read Committed level
- **Durability** - SQL Server transaction logs

## Seeded Data

**Admin User:**
- Email: `admin@library.com`
- Password: `Admin123!`

**Sample Books:**
- The Great Gatsby, To Kill a Mockingbird, 1984, Pride and Prejudice, The Catcher in the Rye

## Caching

- IMemoryCache for frequently accessed data
- 5-minute expiration
- Automatic invalidation on CUD operations

## Database

Migrations apply automatically on startup. Manual commands:

```bash
cd src/API
dotnet ef database update
dotnet ef migrations add MigrationName
```

## Troubleshooting

**SQL Server Connection Issues:**
1. Verify SQL Server is running
2. Check connection string
3. Enable TCP/IP in SQL Server Configuration Manager

**Port in Use:**
Edit `src/API/Properties/launchSettings.json` and change ports

**Token Expired:**
Tokens expire in 60 minutes. Login again.

## Docker

```bash
docker build -t library-api .
docker run -p 8080:80 -e ConnectionStrings__DefaultConnection="YOUR_CONNECTION" library-api
```

## Technologies

- ASP.NET Core 8.0
- Entity Framework Core 8.0
- SQL Server
- JWT Bearer Authentication
- AutoMapper
- BCrypt.Net
- Swagger/OpenAPI
- Memory Cache

## License

MIT License

---
Built with ❤️ using Clean Architecture and SOLID principles
