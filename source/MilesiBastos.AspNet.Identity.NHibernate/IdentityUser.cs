using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;

namespace MilesiBastos.AspNet.Identity.NHibernate
{
  public class IdentityUser : IUser
  {
    public virtual string Id { get; set; }

    public virtual string UserName { get; set; }

    public virtual string PasswordHash { get; set; }

    public virtual string SecurityStamp { get; set; }

    public virtual ICollection<IdentityUserRole> Roles { get; private set; }

    public virtual ICollection<IdentityUserClaim> Claims { get; private set; }

    public virtual ICollection<IdentityUserLogin> Logins { get; private set; }

    public IdentityUser()
    {
      this.Id = Guid.NewGuid().ToString();
      this.Claims = (ICollection<IdentityUserClaim>) new List<IdentityUserClaim>();
      this.Roles = (ICollection<IdentityUserRole>) new List<IdentityUserRole>();
      this.Logins = (ICollection<IdentityUserLogin>) new List<IdentityUserLogin>();
    }

    public IdentityUser(string userName)
      : this()
    {
      this.UserName = userName;
    }
  }
}
