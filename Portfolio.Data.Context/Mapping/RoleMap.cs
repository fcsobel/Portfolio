using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portfolio.Data.Model;

namespace Portfolio.Data.Context.Mapping
{
    public class RoleMap : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("Roles");
            builder.HasKey(c => c.RoleId);
            builder.Property(c => c.Name).HasMaxLength(100);
            builder.Property(c => c.Description).HasMaxLength(255);

            //builder.HasOne(s => s.Metrics)
            //    .WithMany()
            //    .HasForeignKey(t => t.RoleId);

            //builder
            //    .HasMany(s => s.Users)
            //    .WithMany(c => c.Roles)
            //.Map(cs =>
            //{
            //    cs.MapLeftKey("UserId");
            //    cs.MapRightKey("RoleId");
            //    cs.ToTable("UserAccess");
            //});

            //builder.HasOne(bc => bc.Book)
            //    .WithMany(b => b.BookCategories)
            //    .HasForeignKey(bc => bc.BookId);


            //modelBuilder.Entity<BookCategory>()
            //    .HasOne(bc => bc.Category)
            //    .WithMany(c => c.BookCategories)
            //    .HasForeignKey(bc => bc.CategoryId);

        }
    }
}        