using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Configurations
{
    public class BookConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.ToTable("Books");  // ← Creates "Books" table

            builder.HasKey(b => b.Id);  // ← Primary key

            // Column definitions
            builder.Property(b => b.Title).IsRequired().HasMaxLength(200);
            builder.Property(b => b.Author).IsRequired().HasMaxLength(100);
            builder.Property(b => b.ISBN).IsRequired().HasMaxLength(20);
            // ... more properties

            // ← Creates indexes
            builder.HasIndex(b => b.ISBN).IsUnique().HasDatabaseName("IX_Books_ISBN");
            builder.HasIndex(b => b.Title).HasDatabaseName("IX_Books_Title");
            builder.HasIndex(b => b.Author).HasDatabaseName("IX_Books_Author");
        }
    }
}
