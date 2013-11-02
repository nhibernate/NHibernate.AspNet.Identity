using Microsoft.AspNet.Identity;
using NHibernate.Mapping.ByCode.Conformist;
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

    public class IdentityUserMap : ClassMapping<IdentityUser>
    {
        public IdentityUserMap()
        {
            Table("AspNetUsers");
            Id(x => x.Id, m => m.Generator(new UUIDHexCombGeneratorDef("D")));
            Bag(x => x.Roles, m => m.Table("AspNetUserRoles"));
        }
    }

}
