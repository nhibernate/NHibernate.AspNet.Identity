using Microsoft.AspNet.Identity;
using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MilesiBastos.AspNet.Identity.NHibernate
{
  public class RoleStore<TRole> : IRoleStore<TRole>, IDisposable where TRole : IdentityRole
  {
    private bool _disposed;
    private EntityStore<TRole> _roleStore;

    public DbContext Context { get; private set; }

    public RoleStore()
      : this((DbContext) new IdentityDbContext())
    {
    }

    public RoleStore(DbContext context)
    {
      if (context == null)
        throw new ArgumentNullException("context");
      this.Context = context;
      this._roleStore = new EntityStore<TRole>(context);
    }

    public Task<TRole> FindByIdAsync(string roleId)
    {
      this.ThrowIfDisposed();
      return this._roleStore.GetByIdAsync((object) roleId);
    }

    public Task<TRole> FindByNameAsync(string roleName)
    {
      this.ThrowIfDisposed();
      return Task.FromResult<TRole>(Queryable.FirstOrDefault<TRole>(Queryable.Where<TRole>(this._roleStore.EntitySet, (Expression<Func<TRole, bool>>) (u => u.Name.ToUpper() == roleName.ToUpper()))));
    }

    public virtual async Task CreateAsync(TRole role)
    {
      this.ThrowIfDisposed();
      if ((object) role == null)
        throw new ArgumentNullException("role");
      this._roleStore.Create(role);
      int num = await this.Context.SaveChangesAsync();
    }

    public virtual Task DeleteAsync(TRole role)
    {
      throw new NotSupportedException();
    }

    public virtual async Task UpdateAsync(TRole role)
    {
      this.ThrowIfDisposed();
      if ((object) role == null)
        throw new ArgumentNullException("role");
      int num = await this.Context.SaveChangesAsync();
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
      if (disposing && !this._disposed)
        this.Context.Dispose();
      this._disposed = true;
      this.Context = (DbContext) null;
      this._roleStore = (EntityStore<TRole>) null;
    }
  }
}
