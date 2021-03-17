using System;
using System.Collections.Generic;
using System.Text;

namespace Portfolio.Data.Model
{
	public class Role
	{
        public virtual int RoleId { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual List<UserAccess> UserAccess { get; set; }

        public Role()
        {
            this.UserAccess = new List<UserAccess>();
        }
    }
}
