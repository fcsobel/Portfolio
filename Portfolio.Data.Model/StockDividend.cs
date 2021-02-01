using System;
using System.Collections.Generic;
using System.Text;

namespace Portfolio.Data.Model
{
    public class StockDividend
    {
        public int StockId { get; set; }
        public DateTime Date { get; set; }
        public decimal Dividend { get; set; }
    }
}