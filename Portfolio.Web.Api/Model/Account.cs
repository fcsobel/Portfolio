using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Portfolio.Web.Api.Model
{
    public class Account
    {
        public long AccountId { get; set; }
        public string Name { get; set; }
        public List<Investment> Investments { get; set; }

        public Account()
        {
            this.Investments = new List<Investment>();
        }

        public Account(Portfolio.Data.Model.Account obj) : this()
        {
            this.Hydrate(obj);
        }

        public void Hydrate(Portfolio.Data.Model.Account obj)
        {
            this.AccountId = obj.AccountId;
            this.Name = obj.Name;
            foreach (var item in obj.Investments)
            {
                this.Investments.Add(new Investment(item));
            }
        }
    }
}
