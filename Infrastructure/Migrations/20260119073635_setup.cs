using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class setup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Author = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ISBN = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PublishedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "Id", "Author", "CreatedAt", "ISBN", "PublishedDate", "Title", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "F. Scott Fitzgerald", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "978-0743273565", new DateTime(1925, 4, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "The Great Gatsby", null },
                    { 2, "Harper Lee", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "978-0446310789", new DateTime(1960, 7, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "To Kill a Mockingbird", null },
                    { 3, "George Orwell", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "978-0451524935", new DateTime(1949, 6, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "1984", null },
                    { 4, "Jane Austen", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "978-0141439518", new DateTime(1813, 1, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "Pride and Prejudice", null },
                    { 5, "J.D. Salinger", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "978-0316769488", new DateTime(1951, 7, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "The Catcher in the Rye", null },
                    { 6, "J.R.R. Tolkien", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "978-0547928227", new DateTime(1937, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "The Hobbit", null },
                    { 7, "J.K. Rowling", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "978-0747532699", new DateTime(1997, 6, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "Harry Potter and the Philosopher's Stone", null },
                    { 8, "J.R.R. Tolkien", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "978-0618640157", new DateTime(1954, 7, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), "The Lord of the Rings", null },
                    { 9, "George Orwell", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "978-0451526342", new DateTime(1945, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Animal Farm", null },
                    { 10, "Aldous Huxley", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "978-0060850524", new DateTime(1932, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Brave New World", null },
                    { 11, "Dan Brown", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "978-0307474278", new DateTime(2003, 3, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "The Da Vinci Code", null },
                    { 12, "Paulo Coelho", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "978-0062315007", new DateTime(1988, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "The Alchemist", null },
                    { 13, "Suzanne Collins", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "978-0439023481", new DateTime(2008, 9, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "The Hunger Games", null },
                    { 14, "Khaled Hosseini", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "978-1594631931", new DateTime(2003, 5, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), "The Kite Runner", null },
                    { 15, "Gillian Flynn", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "978-0307588371", new DateTime(2012, 5, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "Gone Girl", null },
                    { 16, "Alex Michaelides", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "978-1250301697", new DateTime(2019, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "The Silent Patient", null },
                    { 17, "Frank Herbert", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "978-0441172719", new DateTime(1965, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Dune", null },
                    { 18, "Cormac McCarthy", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "978-0307387899", new DateTime(2006, 9, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "The Road", null },
                    { 19, "James Clear", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "978-0735211292", new DateTime(2018, 10, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "Atomic Habits", null },
                    { 20, "Tara Westover", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "978-0399590504", new DateTime(2018, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Educated", null }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "FirstName", "LastName", "PasswordHash", "Username" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "admin@library.com", "Admin", "User", "$2a$11$HsXzMsj4ss/Gu0I6yrG8TOKwdFdlTZWHWcFjVSCVFfCoQCRhYMqE.", "admin" },
                    { 2, new DateTime(2024, 1, 2, 0, 0, 0, 0, DateTimeKind.Utc), "john.doe@example.com", "John", "Doe", "$2a$11$VNgXXEJV7PMX73MJDg5couAo2ySDO0Cm79XmpbNPzGyjhl82R53/u", "john.doe" },
                    { 3, new DateTime(2024, 1, 3, 0, 0, 0, 0, DateTimeKind.Utc), "jane.smith@example.com", "Jane", "Smith", "$2a$11$Bm64uT1CFYGUG0h3OnPDReWv.Hp51QnnmrfANwJ72jkpus0nUYPLi", "jane.smith" },
                    { 4, new DateTime(2024, 1, 4, 0, 0, 0, 0, DateTimeKind.Utc), "librarian@library.com", "Sarah", "Johnson", "$2a$11$eg4QpGu9iMcRRxIFYc.dROy4DNvbR2L3Sxxteqamoc3pc8J5aLKMG", "librarian1" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Books_Author",
                table: "Books",
                column: "Author");

            migrationBuilder.CreateIndex(
                name: "IX_Books_ISBN",
                table: "Books",
                column: "ISBN",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Books_Title",
                table: "Books",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Books");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
