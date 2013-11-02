using Microsoft.AspNet.Identity;
using System;

namespace MilesiBastos.AspNet.Identity.NHibernate
{
  public class IdentityRole : IRole
  {
    public string Id { get; set; }

    public string Name { get; set; }

    public IdentityRole()
      : this("")
    {
    }

    public IdentityRole(string roleName)
    {
      this.Id = Guid.NewGuid().ToString();
      this.Name = roleName;
    }
  }
}
