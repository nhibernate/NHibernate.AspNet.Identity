using Microsoft.AspNet.Identity;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
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

    public class IdentityRoleMap : ClassMapping<IdentityRole> 
    {
        public IdentityRoleMap()
        {
            Table("AspNetRoles");
            Id(x => x.Id, m => m.Generator(new UUIDHexCombGeneratorDef("D")));
            Property(x => x.Name, m => m.NotNullable(false));
            Bag(x => x.Users, m => m.Table("AspNetUserRoles"));
        }
    }
}
