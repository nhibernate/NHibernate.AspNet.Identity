namespace MilesiBastos.AspNet.Identity.NHibernate
{
  public class IdentityUserClaim
  {
    public virtual int Id { get; set; }

    public virtual string ClaimType { get; set; }

    public virtual string ClaimValue { get; set; }

    public virtual IdentityUser User { get; set; }
  }
}
