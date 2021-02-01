using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Portfolio.Web.Api.Model
{
    public class Investment
    {
        public long AccountId { get; set; }
        public long InvestmentId { get; set; }
        public string Name { get; set; }
        public decimal Quantity { get; set; }
        public decimal Cost { get; set; }

        public int StockId { get; set; }
        public string Ticker { get; set; }
        public decimal Price { get; set; }
        public decimal Dividend { get; set; }
        public double Minutes { get; set; }
        public bool Reload { get; set; }
        public string Tags { set; get; }

        public List<StockDividend> Dividends { get; set; }

        public Investment()
        {
            this.Dividends = new List<StockDividend>();
        }

        public Investment(Portfolio.Data.Model.Investment obj) : this() { this.Hydrate(obj); }

        public void Hydrate(Portfolio.Data.Model.Investment obj)
        {
            this.AccountId = obj.AccountId;
            this.InvestmentId = obj.InvestmentId;
            this.Name = obj.Name;
            this.Quantity = obj.Quantity;
            this.Cost = obj.Cost;
            if (obj.Stock != null)
            {
                this.Hydrate(obj.Stock);
            }
        }

        public void Hydrate(Portfolio.Data.Model.Stock obj)
        {
            this.StockId = obj.StockId;
            this.Name = obj.Name;
            this.Ticker = obj.Ticker;
            this.Price = obj.Price;
            this.Dividend = obj.Dividend;
            this.Minutes = obj.Minutes;
            this.Reload = obj.Reload;

            foreach (var item in obj.Dividends)
            {
                this.Dividends.Add(new StockDividend(item));
            }
        }
    }    
}