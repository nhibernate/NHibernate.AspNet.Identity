namespace MilesiBastos.AspNet.Identity.NHibernate
{
  public class IdentityUserRole
  {
    public virtual string UserId { get; set; }

    public virtual string RoleId { get; set; }

    public virtual IdentityRole Role { get; set; }

    public virtual IdentityUser User { get; set; }
  }
}
