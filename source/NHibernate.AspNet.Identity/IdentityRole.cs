using System.Collections.Generic;
using Microsoft.AspNet.Identity;
using NHibernate.AspNet.Identity.DomainModel;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace NHibernate.AspNet.Identity
{
    public class IdentityRole : EntityWithTypedId<string>, IRole
    {
        public virtual string Name { get; set; }

        public virtual ICollection<IdentityUser> Users { get; protected set; }

        public IdentityRole()
        {
            this.Users = (ICollection<IdentityUser>)new List<IdentityUser>();
        }

        public IdentityRole(string roleName) : this()
        {
            this.Name = roleName;
        }
    }

    public class IdentityRoleMap : ClassMapping<IdentityRole> 
    {
        public IdentityRoleMap()
        {
            this.Table("AspNetRoles");
            this.Id(x => x.Id, m => m.Generator(new UUIDHexCombGeneratorDef("D")));
            this.Property(x => x.Name, map =>
            {
                map.Length(255);
                map.NotNullable(true);
                map.Unique(true);
            });
            this.Bag(x => x.Users, map =>
            {
                map.Table("AspNetUserRoles");
                map.Cascade(Cascade.None);
                map.Key(k => k.Column("RoleId"));
            }, rel => rel.ManyToMany(p => p.Column("UserId")));
        }
    }
}