using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portfolio.Data.Model;

namespace Portfolio.Data.Context.Mapping
{
    public class UserMap : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
            builder.HasKey(c => c.UserId);
            builder.Property(c => c.UserName).HasMaxLength(100);
            builder.Property(c => c.Email).HasMaxLength(100);
            builder.Property(c => c.PasswordHash).HasMaxLength(256);
            
            builder.HasMany(s => s.Accounts)
                .WithOne()
                .HasForeignKey(t => t.UserId);

            builder.HasMany(s => s.LoginProfiles)
                .WithOne(x=>x.User)
                .HasForeignKey(t => t.UserId);
        }
    }
}

        