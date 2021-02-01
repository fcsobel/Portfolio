using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portfolio.Data.Model;

namespace Portfolio.Data.Context.Mapping
{
    public class AccountMap : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.ToTable("Accounts");
            builder.HasKey(c => c.AccountId);
            builder.Property(c => c.Name).HasMaxLength(100);
            builder.Property(c => c.Url).HasMaxLength(100);

            builder.HasMany(s => s.Investments)
                .WithOne(s => s.Account)
                .HasForeignKey(t => t.AccountId);

            //builder.HasMany(s => s.Collections)
            //    .WithOne(s=>s.Account)
            //    .HasForeignKey(t => t.AccountId);
        }
    }
}

        