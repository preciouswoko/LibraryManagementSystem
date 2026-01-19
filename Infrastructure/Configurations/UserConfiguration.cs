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
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");  // ← Creates "Users" table

            builder.HasKey(u => u.Id);  // ← Primary key

            // Column definitions
            builder.Property(u => u.Username).IsRequired().HasMaxLength(50);
            builder.Property(u => u.Email).IsRequired().HasMaxLength(100);
            // ... more properties

            // ← Creates unique indexes
            builder.HasIndex(u => u.Username).IsUnique().HasDatabaseName("IX_Users_Username");
            builder.HasIndex(u => u.Email).IsUnique().HasDatabaseName("IX_Users_Email");
        }
    }
}
