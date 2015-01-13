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
    public class UserStore<TUser> : IUserLoginStore<TUser>, IUserClaimStore<TUser>, IUserRoleStore<TUser>, IUserPasswordStore<TUser>, IUserSecurityStampStore<TUser>, IQueryableUserStore<TUser>, IUserStore<TUser>, IUserLockoutStore<TUser, string>, IUserEmailStore<TUser>, IUserPhoneNumberStore<TUser>, IUserTwoFactorStore<TUser, string>, IDisposable where TUser : IdentityUser
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
            {
                throw new ArgumentNullException("context");
            }

            this.ShouldDisposeSession = true;
            this.Context = context;
        }

        public virtual Task<TUser> FindByIdAsync(string userId)
        {
            this.ThrowIfDisposed();
            //return Task.FromResult(this.Context.Get<TUser>((object)userId));
            return this.GetUserAggregateAsync((TUser u) => u.Id.Equals(userId));
        }

        public virtual Task<TUser> FindByNameAsync(string userName)
        {
            this.ThrowIfDisposed();
            //return Task.FromResult<TUser>(Queryable.FirstOrDefault<TUser>(Queryable.Where<TUser>(this.Context.Query<TUser>(), (Expression<Func<TUser, bool>>)(u => u.UserName.ToUpper() == userName.ToUpper()))));
            return this.GetUserAggregateAsync((TUser u) => u.UserName.ToUpper() == userName.ToUpper());
        }

        public virtual async Task CreateAsync(TUser user)
        {
            this.ThrowIfDisposed();
            if ((object)user == null)
            {
                throw new ArgumentNullException("user");
            }
            using (var transaction = new TransactionScope(TransactionScopeOption.Required))
            {
                await Task.FromResult(this.Context.Save(user));
                transaction.Complete();
            }
        }

        public virtual async Task DeleteAsync(TUser user)
        {
            if ((object)user == null)
            {
                throw new ArgumentNullException("user");
            }
            using (var transaction = new TransactionScope(TransactionScopeOption.Required))
            {
                this.Context.Delete(user);
                transaction.Complete();
                await Task.FromResult(0);
            }
        }

        public virtual async Task UpdateAsync(TUser user)
        {
            this.ThrowIfDisposed();
            if ((object)user == null)
            {
                throw new ArgumentNullException("user");
            }
            using (var transaction = new TransactionScope(TransactionScopeOption.Required))
            {
                this.Context.Update(user);
                transaction.Complete();
                int num = await Task.FromResult(0);
            }
        }

        private void ThrowIfDisposed()
        {
            if (this._disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
        }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize((object)this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && this.Context != null && this.ShouldDisposeSession)
            {
                this.Context.Dispose();
            }
            this._disposed = true;
            this.Context = (ISession)null;
        }

        public virtual async Task<TUser> FindAsync(UserLoginInfo login)
        {
            this.ThrowIfDisposed();
            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            var query = from u in this.Context.Query<TUser>()
                        from l in u.Logins
                        where l.LoginProvider == login.LoginProvider && l.ProviderKey == login.ProviderKey
                        select u;

            TUser entity = await Task.FromResult(query.SingleOrDefault());

            return entity;
        }

        public virtual Task AddLoginAsync(TUser user, UserLoginInfo login)
        {
            this.ThrowIfDisposed();
            if ((object)user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

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
            {
                throw new ArgumentNullException("user");
            }
            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            using (var transaction = new TransactionScope(TransactionScopeOption.Required))
            {
                var info = user.Logins.SingleOrDefault(x => x.LoginProvider == login.LoginProvider && x.ProviderKey == login.ProviderKey);
                if (info != null)
                {
                    user.Logins.Remove(info);
                    this.Context.Update(user);
                }
                transaction.Complete();
            }
            return (Task)Task.FromResult<int>(0);
        }

        public virtual Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user)
        {
            this.ThrowIfDisposed();
            if ((object)user == null)
            {
                throw new ArgumentNullException("user");
            }

            IList<UserLoginInfo> result = new List<UserLoginInfo>();
            foreach (IdentityUserLogin identityUserLogin in (IEnumerable<IdentityUserLogin>)user.Logins)
            {
                result.Add(new UserLoginInfo(identityUserLogin.LoginProvider, identityUserLogin.ProviderKey));
            }

            return Task.FromResult<IList<UserLoginInfo>>(result);
        }

        public virtual Task<IList<Claim>> GetClaimsAsync(TUser user)
        {
            this.ThrowIfDisposed();
            if ((object)user == null)
            {
                throw new ArgumentNullException("user");
            }

            IList<Claim> result = new List<Claim>();
            foreach (IdentityUserClaim identityUserClaim in (IEnumerable<IdentityUserClaim>)user.Claims)
            {
                result.Add(new Claim(identityUserClaim.ClaimType, identityUserClaim.ClaimValue));
            }

            return Task.FromResult<IList<Claim>>(result);
        }

        public virtual Task AddClaimAsync(TUser user, Claim claim)
        {
            this.ThrowIfDisposed();
            if ((object)user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (claim == null)
            {
                throw new ArgumentNullException("claim");
            }

            using (var transaction = new TransactionScope(TransactionScopeOption.Required))
            {
                user.Claims.Add(new IdentityUserClaim()
                {
                    User = user,
                    ClaimType = claim.Type,
                    ClaimValue = claim.Value
                });
                transaction.Complete();
            }

            return (Task)Task.FromResult<int>(0);
        }

        public virtual Task RemoveClaimAsync(TUser user, Claim claim)
        {
            this.ThrowIfDisposed();
            if ((object)user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (claim == null)
            {
                throw new ArgumentNullException("claim");
            }

            using (var transaction = new TransactionScope(TransactionScopeOption.Required))
            {
                foreach (IdentityUserClaim identityUserClaim in Enumerable.ToList<IdentityUserClaim>(Enumerable.Where<IdentityUserClaim>((IEnumerable<IdentityUserClaim>)user.Claims, (Func<IdentityUserClaim, bool>)(uc =>
                {
                    if (uc.ClaimValue == claim.Value)
                    {
                        return uc.ClaimType == claim.Type;
                    }
                    else
                    {
                        return false;
                    }
                }))))
                {
                    user.Claims.Remove(identityUserClaim);
                    this.Context.Delete(identityUserClaim);
                }
                transaction.Complete();
            }
            
            return (Task)Task.FromResult<int>(0);
        }

        public virtual Task AddToRoleAsync(TUser user, string role)
        {
            this.ThrowIfDisposed();
            if ((object)user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (string.IsNullOrWhiteSpace(role))
            {
                throw new ArgumentException(Resources.ValueCannotBeNullOrEmpty, "role");
            }

            using (var transaction = new TransactionScope(TransactionScopeOption.Required))
            {
                IdentityRole identityRole = Queryable.SingleOrDefault<IdentityRole>(this.Context.Query<IdentityRole>(), (Expression<Func<IdentityRole, bool>>)(r => r.Name.ToUpper() == role.ToUpper()));
                if (identityRole == null)
                {
                    throw new InvalidOperationException(string.Format((IFormatProvider)CultureInfo.CurrentCulture, Resources.RoleNotFound, new object[1] { (object)role }));
                }
                user.Roles.Add(identityRole);
                transaction.Complete();
                return (Task)Task.FromResult<int>(0);
            }
        }

        public virtual Task RemoveFromRoleAsync(TUser user, string role)
        {
            this.ThrowIfDisposed();
            if ((object)user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (string.IsNullOrWhiteSpace(role))
            {
                throw new ArgumentException(Resources.ValueCannotBeNullOrEmpty, "role");
            }

            using (var transaction = new TransactionScope(TransactionScopeOption.Required))
            {
                IdentityRole identityUserRole = Enumerable.FirstOrDefault<IdentityRole>(Enumerable.Where<IdentityRole>((IEnumerable<IdentityRole>)user.Roles, (Func<IdentityRole, bool>)(r => r.Name.ToUpper() == role.ToUpper())));
                if (identityUserRole != null)
                {
                    user.Roles.Remove(identityUserRole);
                }
                transaction.Complete();
                return (Task)Task.FromResult<int>(0);
            }
        }

        public virtual Task<IList<string>> GetRolesAsync(TUser user)
        {
            this.ThrowIfDisposed();
            if ((object)user == null)
            {
                throw new ArgumentNullException("user");
            }
            else
            {
                return Task.FromResult<IList<string>>((IList<string>)Enumerable.ToList<string>(Enumerable.Select<IdentityRole, string>((IEnumerable<IdentityRole>)user.Roles, (Func<IdentityRole, string>)(u => u.Name))));
        }
        }

        public virtual Task<bool> IsInRoleAsync(TUser user, string role)
        {
            this.ThrowIfDisposed();
            if ((object)user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (string.IsNullOrWhiteSpace(role))
            {
                throw new ArgumentException(Resources.ValueCannotBeNullOrEmpty, "role");
            }
            else
            {
                return Task.FromResult<bool>(Enumerable.Any<IdentityRole>((IEnumerable<IdentityRole>)user.Roles, (Func<IdentityRole, bool>)(r => r.Name.ToUpper() == role.ToUpper())));
        }
        }

        public virtual Task SetPasswordHashAsync(TUser user, string passwordHash)
        {
            this.ThrowIfDisposed();
            if ((object)user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.PasswordHash = passwordHash;
            return (Task)Task.FromResult<int>(0);
        }

        public virtual Task<string> GetPasswordHashAsync(TUser user)
        {
            this.ThrowIfDisposed();
            if ((object)user == null)
            {
                throw new ArgumentNullException("user");
            }
            else
            {
                return Task.FromResult<string>(user.PasswordHash);
        }
        }

        public virtual Task SetSecurityStampAsync(TUser user, string stamp)
        {
            this.ThrowIfDisposed();
            if ((object)user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.SecurityStamp = stamp;
            return (Task)Task.FromResult<int>(0);
        }

        public virtual Task<string> GetSecurityStampAsync(TUser user)
        {
            this.ThrowIfDisposed();
            if ((object)user == null)
            {
                throw new ArgumentNullException("user");
            }
            else
            {
                return Task.FromResult<string>(user.SecurityStamp);
        }
        }

        public virtual Task<bool> HasPasswordAsync(TUser user)
        {
            return Task.FromResult<bool>(user.PasswordHash != null);
        }

        public IQueryable<TUser> Users
        {
            get
            {
                this.ThrowIfDisposed();
                return this.Context.Query<TUser>();
            }
        }

        public Task<int> GetAccessFailedCountAsync(TUser user)
        {
            this.ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult<int>(user.AccessFailedCount);
        }

        public Task<bool> GetLockoutEnabledAsync(TUser user)
        {
            this.ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult<bool>(user.LockoutEnabled);
        }

        public Task<DateTimeOffset> GetLockoutEndDateAsync(TUser user)
        {
            DateTimeOffset dateTimeOffset;
            this.ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (user.LockoutEndDateUtc.HasValue)
            {
                DateTime? lockoutEndDateUtc = user.LockoutEndDateUtc;
                dateTimeOffset = new DateTimeOffset(DateTime.SpecifyKind(lockoutEndDateUtc.Value, DateTimeKind.Utc));
            }
            else
            {
                dateTimeOffset = new DateTimeOffset();
            }
            return Task.FromResult<DateTimeOffset>(dateTimeOffset);
        }

        public Task<int> IncrementAccessFailedCountAsync(TUser user)
        {
            this.ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.AccessFailedCount = user.AccessFailedCount + 1;
            return Task.FromResult<int>(user.AccessFailedCount);
        }

        public Task ResetAccessFailedCountAsync(TUser user)
        {
            this.ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.AccessFailedCount = 0;
            return Task.FromResult<int>(0);
        }

        public Task SetLockoutEnabledAsync(TUser user, bool enabled)
        {
            this.ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.LockoutEnabled = enabled;
            return Task.FromResult<int>(0);
        }

        public Task SetLockoutEndDateAsync(TUser user, DateTimeOffset lockoutEnd)
        {
            DateTime? nullable;
            this.ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (lockoutEnd == DateTimeOffset.MinValue)
            {
                nullable = null;
            }
            else
            {
                nullable = new DateTime?(lockoutEnd.UtcDateTime);
            }
            user.LockoutEndDateUtc = nullable;
            return Task.FromResult<int>(0);
        }

        public Task<TUser> FindByEmailAsync(string email)
        {
            this.ThrowIfDisposed();
            return this.GetUserAggregateAsync((TUser u) => u.Email.ToUpper() == email.ToUpper());
        }

        public Task<string> GetEmailAsync(TUser user)
        {
            this.ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult<string>(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(TUser user)
        {
            this.ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult<bool>(user.EmailConfirmed);
        }

        public Task SetEmailAsync(TUser user, string email)
        {
            this.ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.Email = email;
            return Task.FromResult<int>(0);
        }

        public Task SetEmailConfirmedAsync(TUser user, bool confirmed)
        {
            this.ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.EmailConfirmed = confirmed;
            return Task.FromResult<int>(0);
        }

        private Task<TUser> GetUserAggregateAsync(Expression<Func<TUser, bool>> filter)
        {
            //return this.Users.Include<TUser, ICollection<TUserRole>>((TUser u) => u.Roles).Include<TUser, ICollection<TUserClaim>>((TUser u) => u.Claims).Include<TUser, ICollection<TUserLogin>>((TUser u) => u.Logins).FirstOrDefaultAsync<TUser>(filter);
            //return Task.Run(() => this.Context.Query<TUser>()
            //                          .FetchMany(p => p.Roles)
            //                          .FetchMany(p => p.Claims)
            //                          .FetchMany(p => p.Logins)
            //                          .FirstOrDefault(filter));
            return Task.Run(() =>
            {
                // no cartesian product
                var user = this.Context.Query<TUser>().Where(filter).FirstOrDefault();
                if (user != null)
                {
                    NHibernateUtil.Initialize(user.Roles);
                    NHibernateUtil.Initialize(user.Claims);
                    NHibernateUtil.Initialize(user.Logins);
                }
                return user;
            });

        }

        public Task<string> GetPhoneNumberAsync(TUser user)
        {
            this.ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult<string>(user.PhoneNumber);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(TUser user)
        {
            this.ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult<bool>(user.PhoneNumberConfirmed);
        }

        public Task SetPhoneNumberAsync(TUser user, string phoneNumber)
        {
            this.ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.PhoneNumber = phoneNumber;
            return Task.FromResult<int>(0);
        }

        public Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed)
        {
            this.ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.PhoneNumberConfirmed = confirmed;
            return Task.FromResult<int>(0);
        }

        public Task<bool> GetTwoFactorEnabledAsync(TUser user)
        {
            this.ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult<bool>(user.TwoFactorEnabled);
        }

        public Task SetTwoFactorEnabledAsync(TUser user, bool enabled)
        {
            this.ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.TwoFactorEnabled = enabled;
            return Task.FromResult<int>(0);
        }
    }
}
