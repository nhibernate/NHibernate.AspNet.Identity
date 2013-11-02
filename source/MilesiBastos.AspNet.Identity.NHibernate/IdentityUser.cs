using Microsoft.AspNet.Identity;
using SharpArch.Domain.DomainModel;
using System;
using System.Collections.Generic;

namespace MilesiBastos.AspNet.Identity.NHibernate
{
    public class IdentityUser : EntityWithTypedId<string>, IUser
    {
        public virtual string UserName { get; set; }

        public virtual string PasswordHash { get; set; }

        public virtual string SecurityStamp { get; set; }

        public virtual ICollection<IdentityRole> Roles { get; private set; }

        public virtual ICollection<IdentityUserClaim> Claims { get; private set; }

        public virtual ICollection<IdentityUserLogin> Logins { get; private set; }

        public IdentityUser()
        {
            this.Claims = (ICollection<IdentityUserClaim>)new List<IdentityUserClaim>();
            this.Roles = (ICollection<IdentityRole>)new List<IdentityRole>();
            this.Logins = (ICollection<IdentityUserLogin>)new List<IdentityUserLogin>();
        }

        public IdentityUser(string userName)
            : this()
        {
            this.UserName = userName;
        }
    }
}
