namespace MilesiBastos.AspNet.Identity.NHibernate
{
    public class IdentityUserLogin
    {
        public virtual string LoginProvider { get; set; }

        public virtual string ProviderKey { get; set; }

        public virtual IdentityUser User { get; set; }
    }
}
