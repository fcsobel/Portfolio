using System;
using System.Collections.Generic;
using System.Text;

namespace Portfolio.Data.Model
{
    public class UserAccess
    {
        public virtual Guid UserId { get; set; }
        public virtual int RoleId { get; set; }
        public virtual User User { get; set; }
        public virtual Role Role { get; set; }
        public UserAccess() { }
    }
}
