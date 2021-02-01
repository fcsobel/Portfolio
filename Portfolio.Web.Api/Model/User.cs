using Microsoft.AspNetCore.Mvc.Formatters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Portfolio.Web.Api.Model
{
    public class User
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public List<Account> Accounts { get; set; }
        public string ImageUrl { get; set; }

        public User()
        {
            this.Accounts = new List<Account>();
        }

        public User(Portfolio.Data.Model.User obj) : this()
        {
            this.Hydrate(obj);
        }

        public void Hydrate(Portfolio.Data.Model.User obj)
        {
            this.UserId = obj.UserId;
            this.UserName = obj.UserName;
            foreach (var item in obj.Accounts)
            {
                this.Accounts.Add(new Account(item));
            }
            if (obj.LoginProfiles.Any())
            {
                this.ImageUrl = obj.LoginProfiles.FirstOrDefault().Picture;
            }
        }
    }
}
