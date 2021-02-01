using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portfolio.Data.Model;

namespace Portfolio.Data.Context.Mapping
{
    public class StockDividendMap : IEntityTypeConfiguration<StockDividend>
    {
        public void Configure(EntityTypeBuilder<StockDividend> builder)
        {
            builder.ToTable("StockDividends");
            builder.HasKey(c => new { c.StockId, c.Date });
            builder.Property(c => c.Date).HasColumnType("date");
            builder.Property(c => c.Dividend).HasColumnType("decimal(19, 4)");
        }
    }
}

        