using Microsoft.AspNet.Identity;
using SharpArch.Domain.DomainModel;
using System.Collections.Generic;

namespace MilesiBastos.AspNet.Identity.NHibernate
{
    public class IdentityRole : EntityWithTypedId<string>, IRole
    {
        public string Name { get; set; }

        public virtual ICollection<IdentityUser> Users { get; private set; }

        public IdentityRole()
        {
            this.Users = (ICollection<IdentityUser>)new List<IdentityUser>();
        }

        public IdentityRole(string roleName)
            : this()
        {
            this.Name = roleName;
        }
    }
}
