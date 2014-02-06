using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNet.Identity;
using NHibernate.AspNet.Identity.Properties;
using NHibernate.Linq;

namespace NHibernate.AspNet.Identity
{
    /// <summary>
    /// Implements IUserStore using NHibernate where TUser is the entity type of the user being stored
    /// </summary>
    /// <typeparam name="TUser"/>
    public class UserStore<TUser> : IUserLoginStore<TUser>, IUserClaimStore<TUser>, IUserRoleStore<TUser>, IUserPasswordStore<TUser>, IUserSecurityStampStore<TUser>, IUserStore<TUser>, IDisposable where TUser : IdentityUser
    {
        private bool _disposed;

        /// <summary>
        /// If true then disposing this object will also dispose (close) the session. False means that external code is responsible for disposing the session.
        /// </summary>
        public bool ShouldDisposeSession { get; set; }

        public ISession Context { get; private set; }

        public UserStore(ISession context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            ShouldDisposeSession = true;
            this.Context = context;
        }

        public virtual Task<TUser> FindByIdAsync(string userId)
        {
            this.ThrowIfDisposed();
            return Task.FromResult(Context.Get<TUser>((object)userId));
        }

        public virtual Task<TUser> FindByNameAsync(string userName)
        {
            this.ThrowIfDisposed();
            return Task.FromResult<TUser>(Queryable.FirstOrDefault<TUser>(Queryable.Where<TUser>(this.Context.Query<TUser>(), (Expression<Func<TUser, bool>>)(u => u.UserName.ToUpper() == userName.ToUpper()))));
        }

        public virtual async Task CreateAsync(TUser user)
        {
            this.ThrowIfDisposed();
            if ((object)user == null)
                throw new ArgumentNullException("user");
            await Task.FromResult(Context.Save(user));
        }

        public virtual Task DeleteAsync(TUser user)
        {
            throw new NotSupportedException();
        }

        public virtual async Task UpdateAsync(TUser user)
        {
            this.ThrowIfDisposed();
            if ((object)user == null)
                throw new ArgumentNullException("user");
            Context.Update(user);
            int num = await Task.FromResult(0);
        }

        private void ThrowIfDisposed()
        {
            if (this._disposed)
                throw new ObjectDisposedException(this.GetType().Name);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize((object)this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && this.Context != null && ShouldDisposeSession)
                this.Context.Dispose();
            this._disposed = true;
            this.Context = (ISession)null;
        }

        public virtual async Task<TUser> FindAsync(UserLoginInfo login)
        {
            this.ThrowIfDisposed();
            if (login == null)
                throw new ArgumentNullException("login");

            var query = from u in this.Context.Query<IdentityUser>()
                        from l in u.Logins
                        where l.LoginProvider == login.LoginProvider && l.ProviderKey == login.ProviderKey
                        select u;

            IdentityUser entity = await Task.FromResult(query.SingleOrDefault());

            return entity as TUser;
        }

        public virtual Task AddLoginAsync(TUser user, UserLoginInfo login)
        {
            this.ThrowIfDisposed();
            if ((object)user == null)
                throw new ArgumentNullException("user");
            if (login == null)
                throw new ArgumentNullException("login");

            using (var transaction = new TransactionScope(TransactionScopeOption.Required))
            {
                user.Logins.Add(new IdentityUserLogin()
                {
                    ProviderKey = login.ProviderKey,
                    LoginProvider = login.LoginProvider
                });

                this.Context.SaveOrUpdate(user);
                transaction.Complete();
            }
            return (Task)Task.FromResult<int>(0);
        }

        public virtual Task RemoveLoginAsync(TUser user, UserLoginInfo login)
        {
            this.ThrowIfDisposed();
            if ((object)user == null)
                throw new ArgumentNullException("user");
            if (login == null)
                throw new ArgumentNullException("login");

            var info = user.Logins.SingleOrDefault(x => x.LoginProvider == login.LoginProvider && x.ProviderKey == login.ProviderKey);
            if (info != null)
            {
                using (var transaction = new TransactionScope(TransactionScopeOption.Required))
                {
                    user.Logins.Remove(info);
                    this.Context.Update(user);
                    transaction.Complete();
                }
            }
            return (Task)Task.FromResult<int>(0);
        }

        public virtual Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user)
        {
            this.ThrowIfDisposed();
            if ((object)user == null)
                throw new ArgumentNullException("user");

            IList<UserLoginInfo> result = (IList<UserLoginInfo>)new List<UserLoginInfo>();
            foreach (IdentityUserLogin identityUserLogin in (IEnumerable<IdentityUserLogin>)user.Logins)
                result.Add(new UserLoginInfo(identityUserLogin.LoginProvider, identityUserLogin.ProviderKey));

            return Task.FromResult<IList<UserLoginInfo>>(result);
        }

        public virtual Task<IList<Claim>> GetClaimsAsync(TUser user)
        {
            this.ThrowIfDisposed();
            if ((object)user == null)
                throw new ArgumentNullException("user");

            IList<Claim> result = (IList<Claim>)new List<Claim>();
            foreach (IdentityUserClaim identityUserClaim in (IEnumerable<IdentityUserClaim>)user.Claims)
                result.Add(new Claim(identityUserClaim.ClaimType, identityUserClaim.ClaimValue));

            return Task.FromResult<IList<Claim>>(result);
        }

        public virtual Task AddClaimAsync(TUser user, Claim claim)
        {
            this.ThrowIfDisposed();
            if ((object)user == null)
                throw new ArgumentNullException("user");
            if (claim == null)
                throw new ArgumentNullException("claim");

            user.Claims.Add(new IdentityUserClaim()
            {
                User = (IdentityUser)user,
                ClaimType = claim.Type,
                ClaimValue = claim.Value
            });

            return (Task)Task.FromResult<int>(0);
        }

        public virtual Task RemoveClaimAsync(TUser user, Claim claim)
        {
            this.ThrowIfDisposed();
            if ((object)user == null)
                throw new ArgumentNullException("user");
            if (claim == null)
                throw new ArgumentNullException("claim");

            foreach (IdentityUserClaim identityUserClaim in Enumerable.ToList<IdentityUserClaim>(Enumerable.Where<IdentityUserClaim>((IEnumerable<IdentityUserClaim>)user.Claims, (Func<IdentityUserClaim, bool>)(uc =>
            {
                if (uc.ClaimValue == claim.Value)
                    return uc.ClaimType == claim.Type;
                else
                    return false;
            }))))
            {
                user.Claims.Remove(identityUserClaim);
                this.Context.Delete(identityUserClaim);
            }

            return (Task)Task.FromResult<int>(0);
        }

        public virtual Task AddToRoleAsync(TUser user, string role)
        {
            this.ThrowIfDisposed();
            if ((object)user == null)
                throw new ArgumentNullException("user");
            if (string.IsNullOrWhiteSpace(role))
                throw new ArgumentException(Resources.ValueCannotBeNullOrEmpty, "role");

            IdentityRole identityRole = Queryable.SingleOrDefault<IdentityRole>((IQueryable<IdentityRole>)this.Context.Query<IdentityRole>(), (Expression<Func<IdentityRole, bool>>)(r => r.Name.ToUpper() == role.ToUpper()));
            if (identityRole == null)
            {
                throw new InvalidOperationException(string.Format((IFormatProvider)CultureInfo.CurrentCulture, Resources.RoleNotFound, new object[1] { (object)role }));
            }
            else
            {
                user.Roles.Add(identityRole);
                return (Task)Task.FromResult<int>(0);
            }
        }

        public virtual Task RemoveFromRoleAsync(TUser user, string role)
        {
            this.ThrowIfDisposed();
            if ((object)user == null)
                throw new ArgumentNullException("user");
            if (string.IsNullOrWhiteSpace(role))
                throw new ArgumentException(Resources.ValueCannotBeNullOrEmpty, "role");

            IdentityRole identityUserRole = Enumerable.FirstOrDefault<IdentityRole>(Enumerable.Where<IdentityRole>((IEnumerable<IdentityRole>)user.Roles, (Func<IdentityRole, bool>)(r => r.Name.ToUpper() == role.ToUpper())));
            if (identityUserRole != null)
            {
                user.Roles.Remove(identityUserRole);
                this.Context.Delete(identityUserRole);
            }

            return (Task)Task.FromResult<int>(0);
        }

        public virtual Task<IList<string>> GetRolesAsync(TUser user)
        {
            this.ThrowIfDisposed();
            if ((object)user == null)
                throw new ArgumentNullException("user");
            else
                return Task.FromResult<IList<string>>((IList<string>)Enumerable.ToList<string>(Enumerable.Select<IdentityRole, string>((IEnumerable<IdentityRole>)user.Roles, (Func<IdentityRole, string>)(u => u.Name))));
        }

        public virtual Task<bool> IsInRoleAsync(TUser user, string role)
        {
            this.ThrowIfDisposed();
            if ((object)user == null)
                throw new ArgumentNullException("user");
            if (string.IsNullOrWhiteSpace(role))
                throw new ArgumentException(Resources.ValueCannotBeNullOrEmpty, "role");
            else
                return Task.FromResult<bool>(Enumerable.Any<IdentityRole>((IEnumerable<IdentityRole>)user.Roles, (Func<IdentityRole, bool>)(r => r.Name.ToUpper() == role.ToUpper())));
        }

        public Task SetPasswordHashAsync(TUser user, string passwordHash)
        {
            this.ThrowIfDisposed();
            if ((object)user == null)
                throw new ArgumentNullException("user");
            user.PasswordHash = passwordHash;
            return (Task)Task.FromResult<int>(0);
        }

        public Task<string> GetPasswordHashAsync(TUser user)
        {
            this.ThrowIfDisposed();
            if ((object)user == null)
                throw new ArgumentNullException("user");
            else
                return Task.FromResult<string>(user.PasswordHash);
        }

        public Task SetSecurityStampAsync(TUser user, string stamp)
        {
            this.ThrowIfDisposed();
            if ((object)user == null)
                throw new ArgumentNullException("user");
            user.SecurityStamp = stamp;
            return (Task)Task.FromResult<int>(0);
        }

        public Task<string> GetSecurityStampAsync(TUser user)
        {
            this.ThrowIfDisposed();
            if ((object)user == null)
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
