using System;
using System.Collections.Generic;
using System.Text;

namespace Portfolio.Data.Model
{
    public class Stock
    {
        public int StockId { get; set; }
        public string Ticker { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal Dividend { get; set; }
        public List<StockPrice> PriceHistory { get; set; }
        public List<StockDividend> Dividends { get; set; }

        // Price Target
        public DateTime PriceTargetUpdated { get; set; }
        public decimal PriceTargetAverage { get; set; }
        public decimal PriceTargetHigh { get; set; }
        public decimal PriceTargetLow { get; set; }
        public int PriceTargetAnalysts { get; set; }

        public decimal? DividendGrowthRate5Y { get; set; }
        public decimal? DividendPerShare5Y { get; set; }
        public decimal? DividendPerShareAnnual { get; set; }
        public decimal? DividendYield5Y { get; set; }
        public decimal? DividendYieldIndicatedAnnual { get; set; }
        public decimal? DividendsPerShareTTM { get; set; }
        public decimal? High_52Week { get; set; }
        public DateTime? HighDate_52Week { get; set; }
        public decimal? Low_52Week { get; set; }
        public DateTime? LowDate_52Week { get; set; }
        public decimal? AverageTradingVolume10Day { get; set; }
        public decimal? PriceReturnDaily13Week { get; set; }
        public decimal? PriceReturnDaily26Week { get; set; }

        public DateTime? CacheDate { get; set; }
        public double Minutes { get { return this.CacheDate != null ? DateTime.Now.Subtract(this.CacheDate.Value).TotalMinutes : 9999; } }
        public bool Reload { get { return this.Minutes > 60; } }

        public Stock()
        {
            this.PriceHistory = new List<StockPrice>();
            this.Dividends = new List<StockDividend>();
        }
    }
}