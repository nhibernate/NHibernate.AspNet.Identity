using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using NHibernate.Linq;

namespace NHibernate.AspNet.Identity
{
    public class RoleStore<TRole> : IQueryableRoleStore<TRole>, IRoleStore<TRole>, IDisposable where TRole : IdentityRole
    {
        private bool _disposed;

        /// <summary>
        /// If true then disposing this object will also dispose (close) the session. False means that external code is responsible for disposing the session.
        /// </summary>
        public bool ShouldDisposeSession { get; set; }

        public ISession Context { get; private set; }

        public RoleStore(ISession context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            ShouldDisposeSession = true;
            this.Context = context;
        }

        public virtual Task<TRole> FindByIdAsync(string roleId)
        {
            this.ThrowIfDisposed();
            return Task.FromResult(Context.Get<TRole>((object)roleId));
        }

        public virtual Task<TRole> FindByNameAsync(string roleName)
        {
            this.ThrowIfDisposed();
            return Task.FromResult<TRole>(Queryable.FirstOrDefault<TRole>(Queryable.Where<TRole>(this.Context.Query<TRole>(), (Expression<Func<TRole, bool>>)(u => u.Name.ToUpper() == roleName.ToUpper()))));
        }

        public virtual async Task CreateAsync(TRole role)
        {
            this.ThrowIfDisposed();
            if ((object)role == null)
                throw new ArgumentNullException("role");
            Context.Save(role);
            Context.Flush();
            await Task.FromResult(0);
        }

        public virtual async Task DeleteAsync(TRole role)
        {
            this.ThrowIfDisposed();
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }
            Context.Delete(role);
            Context.Flush();
            await Task.FromResult(0);
        }

        public virtual async Task UpdateAsync(TRole role)
        {
            this.ThrowIfDisposed();
            if ((object)role == null)
                throw new ArgumentNullException("role");
            Context.Update(role);
            Context.Flush();
            await Task.FromResult(0);
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
            if (disposing && !this._disposed && ShouldDisposeSession)
                this.Context.Dispose();
            this._disposed = true;
            this.Context = (ISession)null;
        }

        public IQueryable<TRole> Roles
        {
            get
            {
                this.ThrowIfDisposed();
                return this.Context.Query<TRole>();
            }
        }
    }
}
