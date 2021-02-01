using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portfolio.Data.Model;

namespace Portfolio.Data.Context.Mapping
{
    public class StockMap : IEntityTypeConfiguration<Stock>
    {
        public void Configure(EntityTypeBuilder<Stock> builder)
        {
            builder.ToTable("Stocks");
            builder.HasKey(c => c.StockId);
            builder.Property(c => c.Ticker).HasMaxLength(10);
            builder.Property(c => c.Name).HasMaxLength(100);
            builder.Property(c => c.Price).HasColumnType("decimal(19, 4)");
            builder.Property(c => c.Dividend).HasColumnType("decimal(19, 5)");

            builder.Property(c => c.PriceTargetAverage).HasColumnType("decimal(19, 5)");
            builder.Property(c => c.PriceTargetHigh).HasColumnType("decimal(19, 5)");
            builder.Property(c => c.PriceTargetLow).HasColumnType("decimal(19, 5)");

             //public DateTime PriceTargetUpdated { get; set; }
            //public int PriceTargetAnalysts { get; set; }

            builder.Property(c => c.DividendGrowthRate5Y).HasColumnType("decimal(19, 5)");
            builder.Property(c => c.DividendPerShare5Y).HasColumnType("decimal(19, 5)");
            builder.Property(c => c.DividendPerShareAnnual).HasColumnType("decimal(19, 5)");
            builder.Property(c => c.DividendYield5Y).HasColumnType("decimal(19, 5)");
            builder.Property(c => c.DividendYieldIndicatedAnnual).HasColumnType("decimal(19, 5)");
            builder.Property(c => c.DividendsPerShareTTM).HasColumnType("decimal(19, 5)");
            builder.Property(c => c.High_52Week).HasColumnType("decimal(19, 5)");
            builder.Property(c => c.Low_52Week).HasColumnType("decimal(19, 5)");
            builder.Property(c => c.AverageTradingVolume10Day).HasColumnType("decimal(19, 5)");
            builder.Property(c => c.PriceReturnDaily13Week).HasColumnType("decimal(19, 5)");
            builder.Property(c => c.PriceReturnDaily26Week).HasColumnType("decimal(19, 5)");

            //builder.HasOne(s => s.Metrics)
            //    .WithMany()
            //    .HasForeignKey(t => t.StockId);

            builder.HasMany(s => s.PriceHistory)
                .WithOne()
                .HasForeignKey(t => t.StockId);

            builder.HasMany(s => s.Dividends)
                .WithOne()
                .HasForeignKey(t => t.StockId);
        }
    }
}        