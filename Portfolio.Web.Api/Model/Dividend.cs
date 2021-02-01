using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Portfolio.Web.Api.Model
{
    public class StockDividend
    {
        public DateTime Date { get; set; }
        public decimal Dividend { get; set; }

        public StockDividend()
        { 
        }

        public StockDividend(Portfolio.Data.Model.StockDividend obj) : this()
        {
            this.Hydrate(obj);
        }

        public void Hydrate(Portfolio.Data.Model.StockDividend obj)
        {
            this.Date = obj.Date;
            this.Dividend = obj.Dividend;
        }
    }
}
