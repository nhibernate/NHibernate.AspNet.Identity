using NHibernate.Mapping.ByCode.Conformist;
using SharpArch.Domain.DomainModel;

namespace MilesiBastos.AspNet.Identity.NHibernate
{
    public class IdentityUserLogin : ValueObject
    {
        public virtual string LoginProvider { get; set; }

        public virtual string ProviderKey { get; set; }

        public virtual IdentityUser User { get; set; }
    }

    public class IdentityUserLoginMap : ClassMapping<IdentityUserLogin>
    {
        public IdentityUserLoginMap()
        {
            Table("AspNetUserLogins");
            ComposedId(m => {
                m.ManyToOne(x => x.User, c => c.Column("UserId"));
                m.Property(x => x.LoginProvider);
                m.Property(x => x.ProviderKey);
            });
        }
    }

}
