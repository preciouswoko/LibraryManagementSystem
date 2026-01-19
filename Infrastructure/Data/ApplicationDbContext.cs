using Domain.Entities;
using Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Books => Set<Book>();
        public DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new BookConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());

            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // ← Seed Users
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    Email = "admin@library.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!", 11),
                    FirstName = "Admin",
                    LastName = "User",
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new User
                {
                    Id = 2,
                    Username = "john.doe",
                    Email = "john.doe@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("User123!", 11),
                    FirstName = "John",
                    LastName = "Doe",
                    CreatedAt = new DateTime(2024, 1, 2, 0, 0, 0, DateTimeKind.Utc)
                },
                new User
                {
                    Id = 3,
                    Username = "jane.smith",
                    Email = "jane.smith@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!", 11),
                    FirstName = "Jane",
                    LastName = "Smith",
                    CreatedAt = new DateTime(2024, 1, 3, 0, 0, 0, DateTimeKind.Utc)
                },
                new User
                {
                    Id = 4,
                    Username = "librarian1",
                    Email = "librarian@library.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Lib123!", 11),
                    FirstName = "Sarah",
                    LastName = "Johnson",
                    CreatedAt = new DateTime(2024, 1, 4, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            // ← Seed Books (20 diverse books)
            modelBuilder.Entity<Book>().HasData(
                new Book
                {
                    Id = 1,
                    Title = "The Great Gatsby",
                    Author = "F. Scott Fitzgerald",
                    ISBN = "978-0743273565",
                    PublishedDate = new DateTime(1925, 4, 10),
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Book
                {
                    Id = 2,
                    Title = "To Kill a Mockingbird",
                    Author = "Harper Lee",
                    ISBN = "978-0446310789",
                    PublishedDate = new DateTime(1960, 7, 11),
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Book
                {
                    Id = 3,
                    Title = "1984",
                    Author = "George Orwell",
                    ISBN = "978-0451524935",
                    PublishedDate = new DateTime(1949, 6, 8),
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Book
                {
                    Id = 4,
                    Title = "Pride and Prejudice",
                    Author = "Jane Austen",
                    ISBN = "978-0141439518",
                    PublishedDate = new DateTime(1813, 1, 28),
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Book
                {
                    Id = 5,
                    Title = "The Catcher in the Rye",
                    Author = "J.D. Salinger",
                    ISBN = "978-0316769488",
                    PublishedDate = new DateTime(1951, 7, 16),
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Book
                {
                    Id = 6,
                    Title = "The Hobbit",
                    Author = "J.R.R. Tolkien",
                    ISBN = "978-0547928227",
                    PublishedDate = new DateTime(1937, 9, 21),
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Book
                {
                    Id = 7,
                    Title = "Harry Potter and the Philosopher's Stone",
                    Author = "J.K. Rowling",
                    ISBN = "978-0747532699",
                    PublishedDate = new DateTime(1997, 6, 26),
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Book
                {
                    Id = 8,
                    Title = "The Lord of the Rings",
                    Author = "J.R.R. Tolkien",
                    ISBN = "978-0618640157",
                    PublishedDate = new DateTime(1954, 7, 29),
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Book
                {
                    Id = 9,
                    Title = "Animal Farm",
                    Author = "George Orwell",
                    ISBN = "978-0451526342",
                    PublishedDate = new DateTime(1945, 8, 17),
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Book
                {
                    Id = 10,
                    Title = "Brave New World",
                    Author = "Aldous Huxley",
                    ISBN = "978-0060850524",
                    PublishedDate = new DateTime(1932, 1, 1),
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Book
                {
                    Id = 11,
                    Title = "The Da Vinci Code",
                    Author = "Dan Brown",
                    ISBN = "978-0307474278",
                    PublishedDate = new DateTime(2003, 3, 18),
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Book
                {
                    Id = 12,
                    Title = "The Alchemist",
                    Author = "Paulo Coelho",
                    ISBN = "978-0062315007",
                    PublishedDate = new DateTime(1988, 1, 1),
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Book
                {
                    Id = 13,
                    Title = "The Hunger Games",
                    Author = "Suzanne Collins",
                    ISBN = "978-0439023481",
                    PublishedDate = new DateTime(2008, 9, 14),
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Book
                {
                    Id = 14,
                    Title = "The Kite Runner",
                    Author = "Khaled Hosseini",
                    ISBN = "978-1594631931",
                    PublishedDate = new DateTime(2003, 5, 29),
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Book
                {
                    Id = 15,
                    Title = "Gone Girl",
                    Author = "Gillian Flynn",
                    ISBN = "978-0307588371",
                    PublishedDate = new DateTime(2012, 5, 24),
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Book
                {
                    Id = 16,
                    Title = "The Silent Patient",
                    Author = "Alex Michaelides",
                    ISBN = "978-1250301697",
                    PublishedDate = new DateTime(2019, 2, 5),
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Book
                {
                    Id = 17,
                    Title = "Dune",
                    Author = "Frank Herbert",
                    ISBN = "978-0441172719",
                    PublishedDate = new DateTime(1965, 8, 1),
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Book
                {
                    Id = 18,
                    Title = "The Road",
                    Author = "Cormac McCarthy",
                    ISBN = "978-0307387899",
                    PublishedDate = new DateTime(2006, 9, 26),
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Book
                {
                    Id = 19,
                    Title = "Atomic Habits",
                    Author = "James Clear",
                    ISBN = "978-0735211292",
                    PublishedDate = new DateTime(2018, 10, 16),
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Book
                {
                    Id = 20,
                    Title = "Educated",
                    Author = "Tara Westover",
                    ISBN = "978-0399590504",
                    PublishedDate = new DateTime(2018, 2, 20),
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );
        }
    }
}