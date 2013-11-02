using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

namespace MilesiBastos.AspNet.Identity.NHibernate
{
  internal class EntityStore<TEntity> where TEntity : class
  {
    public DbContext Context { get; private set; }

    public IQueryable<TEntity> EntitySet
    {
      get
      {
        return (IQueryable<TEntity>) this.DbEntitySet;
      }
    }

    public DbSet<TEntity> DbEntitySet { get; private set; }

    public EntityStore(DbContext context)
    {
      if (context == null)
        throw new ArgumentNullException("context");
      this.Context = context;
      this.DbEntitySet = (DbSet<TEntity>) context.Set<TEntity>();
    }

    public virtual Task<TEntity> GetByIdAsync(object id)
    {
      return this.DbEntitySet.FindAsync(new object[1]
      {
        id
      });
    }

    public void Create(TEntity entity)
    {
      this.DbEntitySet.Add(entity);
    }

    public void Delete(TEntity entity)
    {
      ((DbEntityEntry<TEntity>) this.Context.Entry<TEntity>((M0) entity)).set_State((EntityState) 8);
    }
  }
}
