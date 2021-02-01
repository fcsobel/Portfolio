using System;
using System.Collections.Generic;
using System.Text;

namespace Portfolio.Data.Model
{
    public class StockPrice
    {
        public int StockId { get; set; }
        public DateTime Date { get; set; }
        public decimal Open { get; set; }
        public decimal Close { get; set; }
        public decimal Low { get; set; }
        public decimal High { get; set; }
        public long Volume { get; set; }
    }
}
