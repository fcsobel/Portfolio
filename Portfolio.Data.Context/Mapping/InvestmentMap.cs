using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portfolio.Data.Model;

namespace Portfolio.Data.Context.Mapping
{
    public class InvestmentMap : IEntityTypeConfiguration<Investment>
    {
        public void Configure(EntityTypeBuilder<Investment> builder)
        {
            builder.ToTable("Investments");
            builder.HasKey(c => c.InvestmentId);
            builder.Property(c => c.Name).HasMaxLength(200);

            builder.Property(c => c.Cost).HasColumnType("decimal(19, 2)");
            builder.Property(c => c.Quantity).HasColumnType("decimal(19, 3)");

            builder.HasOne(s => s.Stock)
                .WithMany()
                .HasForeignKey(t => t.StockId);
        }
    }
}

        