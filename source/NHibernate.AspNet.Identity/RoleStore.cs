using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using NHibernate.Linq;

namespace NHibernate.AspNet.Identity
{
    public class RoleStore<TRole> : RoleStore<TRole, ISession, string>
        where TRole : IdentityRole<string>
    {
        public RoleStore(ISession context) : base(context) { }
    }

    public class RoleStore<TRole, TContext> : RoleStore<TRole, TContext, string>
        where TRole : IdentityRole<string>
        where TContext : ISession
    {
        public RoleStore(TContext context) : base(context) { }
    }

    public class RoleStore<TRole, TContext, TKey> :
        IQueryableRoleStore<TRole, TKey>
        // TODO: IRoleClaimStore<TRole>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
        where TContext : ISession
    {
        public RoleStore(TContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            Context = context;
            AutoSaveChanges = true;
        }

        private bool _disposed;

        public TContext Context { get; private set; }

        /// <summary>
        ///     If true will call SaveChanges after CreateAsync/UpdateAsync/DeleteAsync
        /// </summary>
        public bool AutoSaveChanges { get; set; }

        private async Task SaveChanges(CancellationToken cancellationToken)
        {
            if (AutoSaveChanges)
            {
                Context.Flush();
            }
            await Task.FromResult(0);
        }

        public async virtual Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }
            Context.Save(role);
            await SaveChanges(cancellationToken);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }
            //Context.Attach(role);
            role.ConcurrencyStamp = Guid.NewGuid().ToString();
            Context.Update(role);
            try
            {
                await SaveChanges(cancellationToken);
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(ex.Message);
            }
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }
            Context.Delete(role);
            try
            {
                await SaveChanges(cancellationToken);
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(ex.Message);
            }
            return IdentityResult.Success;
        }

        public Task<string> GetRoleIdAsync(TRole role, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }
            return Task.FromResult(ConvertIdToString(role.Id));
        }

        public Task<string> GetRoleNameAsync(TRole role, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }
            return Task.FromResult(role.Name);
        }

        public Task SetRoleNameAsync(TRole role, string roleName, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }
            role.Name = roleName;
            return Task.FromResult(0);
        }

        public virtual TKey ConvertIdFromString(string id)
        {
            if (id == null)
            {
                return default(TKey);
            }
            return (TKey)Convert.ChangeType(id, typeof(TKey));
        }

        public virtual string ConvertIdToString(TKey id)
        {
            if (id.Equals(default(TKey)))
            {
                return null;
            }
            return id.ToString();
        }

        /// <summary>
        ///     Find a role by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual Task<TRole> FindByIdAsync(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            var roleId = ConvertIdFromString(id);
            //return Roles.FirstOrDefaultAsync(u => u.Id.Equals(roleId), cancellationToken);
            return Task.FromResult(Context.Get<TRole>(roleId));
        }

        /// <summary>
        ///     Find a role by normalized name
        /// </summary>
        /// <param name="normalizedName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual Task<TRole> FindByNameAsync(string normalizedName, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            //return Roles.FirstOrDefaultAsync(r => r.NormalizedName == normalizedName, cancellationToken);
            return Task.FromResult<TRole>(Roles.FirstOrDefault(r => r.NormalizedName == normalizedName));
        }

        public virtual Task<string> GetNormalizedRoleNameAsync(TRole role, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }
            return Task.FromResult(role.NormalizedName);
        }

        public virtual Task SetNormalizedRoleNameAsync(TRole role, string normalizedName, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }
            role.NormalizedName = normalizedName;
            return Task.FromResult(0);
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        /// <summary>
        ///     Dispose the store
        /// </summary>
        public void Dispose()
        {
            _disposed = true;
        }

        public async Task<IList<Claim>> GetClaimsAsync(TRole role, CancellationToken cancellationToken = default(CancellationToken))
        {
            ThrowIfDisposed();
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }
            var claims = role.Claims.Select(c => new Claim(c.ClaimType, c.ClaimValue)).ToList();
            return await Task.FromResult(claims);
            //return await RoleClaims.Where(rc => rc.RoleId.Equals(role.Id)).Select(c => new Claim(c.ClaimType, c.ClaimValue)).ToListAsync(cancellationToken);
        }

        public Task AddClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken = default(CancellationToken))
        {
            ThrowIfDisposed();
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }
            if (claim == null)
            {
                throw new ArgumentNullException("claim");
            }

            role.Claims.Add(new IdentityRoleClaim<TKey> { RoleId = role.Id, ClaimType = claim.Type, ClaimValue = claim.Value });

            return Task.FromResult(false);
        }

        public Task RemoveClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken = default(CancellationToken))
        {
            ThrowIfDisposed();
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }
            if (claim == null)
            {
                throw new ArgumentNullException("claim");
            }
            var claims = role.Claims.Where(uc => uc.ClaimValue == claim.Value && uc.ClaimType == claim.Type).ToList();
            foreach (var c in claims)
            {
                role.Claims.Remove(c);
            }

            return Task.FromResult<int>(0);
        }

        public IQueryable<TRole> Roles
        {
            get { return Context.Query<TRole>(); }
        }

        private IQueryable<IdentityRoleClaim<TKey>> RoleClaims { get { return Context.Query<IdentityRoleClaim<TKey>>(); } }

        public Task CreateAsync(TRole role)
        {
            return CreateAsync(role, default(CancellationToken));
        }

        public Task DeleteAsync(TRole role)
        {
            return DeleteAsync(role, default(CancellationToken));
        }

        public Task<TRole> FindByIdAsync(TKey roleId)
        {
            return FindByIdAsync(ConvertIdToString(roleId), default(CancellationToken));
        }

        public Task<TRole> FindByNameAsync(string roleName)
        {
            return FindByNameAsync(roleName, default(CancellationToken));
        }

        public Task UpdateAsync(TRole role)
        {
            return UpdateAsync(role, default(CancellationToken));
        }
    }
}
