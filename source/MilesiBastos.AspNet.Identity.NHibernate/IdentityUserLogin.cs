using NHibernate.Mapping.ByCode.Conformist;

namespace MilesiBastos.AspNet.Identity.NHibernate
{
    public class IdentityUserLogin
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
                m.Property(x => x.User);
                m.Property(x => x.LoginProvider);
                m.Property(x => x.ProviderKey);
            });
        }
    }

}
