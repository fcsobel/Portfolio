using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portfolio.Data.Model;

namespace Portfolio.Data.Context.Mapping
{
    public class StockPriceMap : IEntityTypeConfiguration<StockPrice>
    {
        public void Configure(EntityTypeBuilder<StockPrice> builder)
        {
            builder.ToTable("StockPrice");
            builder.HasKey(c =>  new { c.StockId, c.Date });
            builder.Property(c => c.Date).HasColumnType("date");
            builder.Property(c => c.Open).HasColumnType("decimal(19, 4)");
            builder.Property(c => c.Close).HasColumnType("decimal(19, 4)");
            builder.Property(c => c.High).HasColumnType("decimal(19, 4)");
            builder.Property(c => c.Low).HasColumnType("decimal(19, 4)");
        }
    }
}        