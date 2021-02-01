using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Portfolio.Web.App.Models
{
    public class LoginModel
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberLogin { get; set; }
    }
}
