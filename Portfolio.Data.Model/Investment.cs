using System;
using System.Collections.Generic;
using System.Text;

namespace Portfolio.Data.Model
{
    public enum InvestmentTypeEnum { Stock, MutualFund, Property }
    public class Investment
    {
        public long InvestmentId { get; set; }
        public long AccountId { get; set; }
        public string Name { get; set; }
        public decimal Quantity { get; set; }
        public decimal Cost { get; set; }
        public int StockId { get; set; }
        public bool ReInvest { get; set; }
        public DateTime PurchaseDate { get; set; }
        public InvestmentTypeEnum InvestmentType { get; set; }

        public Account Account { get; set; }
        public Stock Stock { get; set; }
        
        public Investment()
        {

        }
    }
}
