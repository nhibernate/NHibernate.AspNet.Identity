using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MilesiBastos.AspNet.Identity.NHibernate
{
  /// <summary>
  /// Implements IUserStore using EntityFramework where TUser is the entity type of the user being stored
  /// </summary>
  /// <typeparam name="TUser"/>
  public class UserStore<TUser> : IUserLoginStore<TUser>, IUserClaimStore<TUser>, IUserRoleStore<TUser>, IUserPasswordStore<TUser>, IUserSecurityStampStore<TUser>, IUserStore<TUser>, IDisposable where TUser : IdentityUser
  {
    private bool _disposed;
    private EntityStore<TUser> _userStore;
    private EntityStore<IdentityRole> _roleStore;
    private IDbSet<IdentityUserRole> _userRoles;
    private IDbSet<IdentityUserClaim> _userClaims;
    private IDbSet<IdentityUserLogin> _logins;

    public DbContext Context { get; private set; }

    public bool DisposeContext { get; set; }

    public bool AutoSaveChanges { get; set; }

    public UserStore()
      : this((DbContext) new IdentityDbContext<TUser>())
    {
      this.DisposeContext = true;
    }

    public UserStore(DbContext context)
    {
      if (context == null)
        throw new ArgumentNullException("context");
      this.Context = context;
      this.AutoSaveChanges = true;
      this._userStore = new EntityStore<TUser>(context);
      this._roleStore = new EntityStore<IdentityRole>(context);
      this._logins = (IDbSet<IdentityUserLogin>) context.Set<IdentityUserLogin>();
      this._userClaims = (IDbSet<IdentityUserClaim>) context.Set<IdentityUserClaim>();
      this._userRoles = (IDbSet<IdentityUserRole>) context.Set<IdentityUserRole>();
    }

    private async Task SaveChanges()
    {
      if (this.AutoSaveChanges)
      {
        int num = await this.Context.SaveChangesAsync();
      }
    }

    public virtual Task<TUser> FindByIdAsync(string userId)
    {
      this.ThrowIfDisposed();
      return this._userStore.GetByIdAsync((object) userId);
    }

    public virtual Task<TUser> FindByNameAsync(string userName)
    {
      this.ThrowIfDisposed();
      return Task.FromResult<TUser>(Queryable.FirstOrDefault<TUser>(Queryable.Where<TUser>(this._userStore.EntitySet, (Expression<Func<TUser, bool>>) (u => u.UserName.ToUpper() == userName.ToUpper()))));
    }

    public virtual async Task CreateAsync(TUser user)
    {
      this.ThrowIfDisposed();
      if ((object) user == null)
        throw new ArgumentNullException("user");
      this._userStore.Create(user);
      await this.SaveChanges();
    }

    public virtual Task DeleteAsync(TUser user)
    {
      throw new NotSupportedException();
    }

    public virtual async Task UpdateAsync(TUser user)
    {
      this.ThrowIfDisposed();
      if ((object) user == null)
        throw new ArgumentNullException("user");
      await this.SaveChanges();
    }

    private void ThrowIfDisposed()
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.GetType().Name);
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (this.DisposeContext && disposing && this.Context != null)
        this.Context.Dispose();
      this._disposed = true;
      this.Context = (DbContext) null;
      this._userStore = (EntityStore<TUser>) null;
    }

    public virtual async Task<TUser> FindAsync(UserLoginInfo login)
    {
      this.ThrowIfDisposed();
      if (login == null)
        throw new ArgumentNullException("login");
      IdentityUser entity = await ((Task<IdentityUser>) QueryableExtensions.FirstOrDefaultAsync<IdentityUser>((IQueryable<M0>) Queryable.Select<IdentityUserLogin, IdentityUser>(Queryable.Where<IdentityUserLogin>((IQueryable<IdentityUserLogin>) this._logins, (Expression<Func<IdentityUserLogin, bool>>) (l => l.LoginProvider == login.LoginProvider && l.ProviderKey == login.ProviderKey)), (Expression<Func<IdentityUserLogin, IdentityUser>>) (l => l.User))));
      return entity as TUser;
    }

    public virtual Task AddLoginAsync(TUser user, UserLoginInfo login)
    {
      this.ThrowIfDisposed();
      if ((object) user == null)
        throw new ArgumentNullException("user");
      if (login == null)
        throw new ArgumentNullException("login");
      user.Logins.Add(new IdentityUserLogin()
      {
        User = (IdentityUser) user,
        ProviderKey = login.ProviderKey,
        LoginProvider = login.LoginProvider
      });
      return (Task) Task.FromResult<int>(0);
    }

    public virtual Task RemoveLoginAsync(TUser user, UserLoginInfo login)
    {
      this.ThrowIfDisposed();
      if ((object) user == null)
        throw new ArgumentNullException("user");
      if (login == null)
        throw new ArgumentNullException("login");
      IdentityUserLogin identityUserLogin = Enumerable.SingleOrDefault<IdentityUserLogin>(Enumerable.Where<IdentityUserLogin>((IEnumerable<IdentityUserLogin>) user.Logins, (Func<IdentityUserLogin, bool>) (l =>
      {
        if (l.LoginProvider == login.LoginProvider && l.User == (object) user)
          return l.ProviderKey == login.ProviderKey;
        else
          return false;
      })));
      if (identityUserLogin != null)
      {
        user.Logins.Remove(identityUserLogin);
        this._logins.Remove(identityUserLogin);
      }
      return (Task) Task.FromResult<int>(0);
    }

    public virtual Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user)
    {
      this.ThrowIfDisposed();
      if ((object) user == null)
        throw new ArgumentNullException("user");
      IList<UserLoginInfo> result = (IList<UserLoginInfo>) new List<UserLoginInfo>();
      foreach (IdentityUserLogin identityUserLogin in (IEnumerable<IdentityUserLogin>) user.Logins)
        result.Add(new UserLoginInfo(identityUserLogin.LoginProvider, identityUserLogin.ProviderKey));
      return Task.FromResult<IList<UserLoginInfo>>(result);
    }

    public virtual Task<IList<Claim>> GetClaimsAsync(TUser user)
    {
      this.ThrowIfDisposed();
      if ((object) user == null)
        throw new ArgumentNullException("user");
      IList<Claim> result = (IList<Claim>) new List<Claim>();
      foreach (IdentityUserClaim identityUserClaim in (IEnumerable<IdentityUserClaim>) user.Claims)
        result.Add(new Claim(identityUserClaim.ClaimType, identityUserClaim.ClaimValue));
      return Task.FromResult<IList<Claim>>(result);
    }

    public virtual Task AddClaimAsync(TUser user, Claim claim)
    {
      this.ThrowIfDisposed();
      if ((object) user == null)
        throw new ArgumentNullException("user");
      if (claim == null)
        throw new ArgumentNullException("claim");
      user.Claims.Add(new IdentityUserClaim()
      {
        User = (IdentityUser) user,
        ClaimType = claim.Type,
        ClaimValue = claim.Value
      });
      return (Task) Task.FromResult<int>(0);
    }

    public virtual Task RemoveClaimAsync(TUser user, Claim claim)
    {
      this.ThrowIfDisposed();
      if ((object) user == null)
        throw new ArgumentNullException("user");
      if (claim == null)
        throw new ArgumentNullException("claim");
      foreach (IdentityUserClaim identityUserClaim in Enumerable.ToList<IdentityUserClaim>(Enumerable.Where<IdentityUserClaim>((IEnumerable<IdentityUserClaim>) user.Claims, (Func<IdentityUserClaim, bool>) (uc =>
      {
        if (uc.ClaimValue == claim.Value)
          return uc.ClaimType == claim.Type;
        else
          return false;
      }))))
      {
        user.Claims.Remove(identityUserClaim);
        this._userClaims.Remove(identityUserClaim);
      }
      return (Task) Task.FromResult<int>(0);
    }

    public virtual Task AddToRoleAsync(TUser user, string role)
    {
      this.ThrowIfDisposed();
      if ((object) user == null)
        throw new ArgumentNullException("user");
      if (string.IsNullOrWhiteSpace(role))
        throw new ArgumentException(IdentityResources.ValueCannotBeNullOrEmpty, "role");
      IdentityRole identityRole = Queryable.SingleOrDefault<IdentityRole>((IQueryable<IdentityRole>) this._roleStore.DbEntitySet, (Expression<Func<IdentityRole, bool>>) (r => r.Name.ToUpper() == role.ToUpper()));
      if (identityRole == null)
      {
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, IdentityResources.RoleNotFound, new object[1]
        {
          (object) role
        }));
      }
      else
      {
        user.Roles.Add(new IdentityUserRole()
        {
          User = (IdentityUser) user,
          Role = identityRole
        });
        return (Task) Task.FromResult<int>(0);
      }
    }

    public virtual Task RemoveFromRoleAsync(TUser user, string role)
    {
      this.ThrowIfDisposed();
      if ((object) user == null)
        throw new ArgumentNullException("user");
      if (string.IsNullOrWhiteSpace(role))
        throw new ArgumentException(IdentityResources.ValueCannotBeNullOrEmpty, "role");
      IdentityUserRole identityUserRole = Enumerable.FirstOrDefault<IdentityUserRole>(Enumerable.Where<IdentityUserRole>((IEnumerable<IdentityUserRole>) user.Roles, (Func<IdentityUserRole, bool>) (r => r.Role.Name.ToUpper() == role.ToUpper())));
      if (identityUserRole != null)
      {
        user.Roles.Remove(identityUserRole);
        this._userRoles.Remove(identityUserRole);
      }
      return (Task) Task.FromResult<int>(0);
    }

    public virtual Task<IList<string>> GetRolesAsync(TUser user)
    {
      this.ThrowIfDisposed();
      if ((object) user == null)
        throw new ArgumentNullException("user");
      else
        return Task.FromResult<IList<string>>((IList<string>) Enumerable.ToList<string>(Enumerable.Select<IdentityUserRole, string>((IEnumerable<IdentityUserRole>) user.Roles, (Func<IdentityUserRole, string>) (u => u.Role.Name))));
    }

    public virtual Task<bool> IsInRoleAsync(TUser user, string role)
    {
      this.ThrowIfDisposed();
      if ((object) user == null)
        throw new ArgumentNullException("user");
      if (string.IsNullOrWhiteSpace(role))
        throw new ArgumentException(IdentityResources.ValueCannotBeNullOrEmpty, "role");
      else
        return Task.FromResult<bool>(Enumerable.Any<IdentityUserRole>((IEnumerable<IdentityUserRole>) user.Roles, (Func<IdentityUserRole, bool>) (r => r.Role.Name.ToUpper() == role.ToUpper())));
    }

    public Task SetPasswordHashAsync(TUser user, string passwordHash)
    {
      this.ThrowIfDisposed();
      if ((object) user == null)
        throw new ArgumentNullException("user");
      user.PasswordHash = passwordHash;
      return (Task) Task.FromResult<int>(0);
    }

    public Task<string> GetPasswordHashAsync(TUser user)
    {
      this.ThrowIfDisposed();
      if ((object) user == null)
        throw new ArgumentNullException("user");
      else
        return Task.FromResult<string>(user.PasswordHash);
    }

    public Task SetSecurityStampAsync(TUser user, string stamp)
    {
      this.ThrowIfDisposed();
      if ((object) user == null)
        throw new ArgumentNullException("user");
      user.SecurityStamp = stamp;
      return (Task) Task.FromResult<int>(0);
    }

    public Task<string> GetSecurityStampAsync(TUser user)
    {
      this.ThrowIfDisposed();
      if ((object) user == null)
        throw new ArgumentNullException("user");
      else
        return Task.FromResult<string>(user.SecurityStamp);
    }

    public Task<bool> HasPasswordAsync(TUser user)
    {
      return Task.FromResult<bool>(user.PasswordHash != null);
    }
  }
}
