using System.Collections.Generic;
using Microsoft.AspNet.Identity;
using NHibernate.AspNet.Identity.DomainModel;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace NHibernate.AspNet.Identity
{
    public class IdentityUser : EntityWithTypedId<string>, IUser
    {
        public virtual string UserName { get; set; }

        public virtual string PasswordHash { get; set; }

        public virtual string SecurityStamp { get; set; }

        public virtual ICollection<IdentityRole> Roles { get; protected set; }

        public virtual ICollection<IdentityUserClaim> Claims { get; protected set; }

        public virtual ICollection<IdentityUserLogin> Logins { get; protected set; }

        public IdentityUser()
        {
            this.Roles = (ICollection<IdentityRole>)new List<IdentityRole>();
            this.Claims = (ICollection<IdentityUserClaim>)new List<IdentityUserClaim>();
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

            Property(x => x.UserName);
            Property(x => x.PasswordHash);
            Property(x => x.SecurityStamp);

            Bag(x => x.Claims, map => { map.Key(k => k.Column("UserId")); }, rel => { rel.OneToMany(); });

            Set(x => x.Logins, cam =>
            {
                cam.Table("AspNetUserLogins");
                cam.Key(km => km.Column("UserId"));
                cam.Cascade(Mapping.ByCode.Cascade.All.Include(Cascade.DeleteOrphans));
            },
            map =>
            {
                map.Component(comp => {
                    comp.Property(p => p.LoginProvider);
                    comp.Property(p => p.ProviderKey);
                });
            });

            Bag(x => x.Roles, map => {
                map.Table("AspNetUserRoles");
                map.Key(k => k.Column("UserId"));
            }, rel => rel.ManyToMany(p => p.Column("RoleId")));
        }
    }

}
