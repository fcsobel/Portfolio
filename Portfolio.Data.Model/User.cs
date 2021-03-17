using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Portfolio.Data.Model
{
	public class User
	{
        public virtual Guid UserId { get; set; }
        public virtual string Email { get; set; }
        public virtual string UserName { get; set; }
        public virtual string PasswordHash { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual List<Account> Accounts { get; set; }
        public virtual List<LoginProfile> LoginProfiles { get; set; }
        public virtual List<UserAccess> UserAccess { get; set; }

        [NotMapped]
        public virtual List<Role> Roles { get { return this.UserAccess.Select(x => x.Role).ToList(); } }

        public User()
        {
            this.Accounts = new List<Account>();
            this.LoginProfiles = new List<LoginProfile>();
            this.UserAccess = new List<UserAccess>();
        }
    }
}
