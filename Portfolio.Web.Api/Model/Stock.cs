using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Portfolio.Web.Api.Model
{
    public class Stock
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}

