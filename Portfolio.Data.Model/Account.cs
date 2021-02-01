using System;
using System.Collections.Generic;
using System.Text;

namespace Portfolio.Data.Model
{
	public class Account
	{
        public long AccountId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public Guid UserId { get; set; }
        public User Owner { get; set; }
        public List<Investment> Investments { get; set; }

        public Account()
        {
            this.Investments = new List<Investment>();
        }
    }
}
