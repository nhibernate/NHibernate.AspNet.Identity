using System.Collections.Generic;

namespace NHibernate
{
    public interface ISessionResolver
    {
        IEnumerable<ISessionFactory> GetAllFactories();
        ISessionFactory GetFactoryFor<TEntity>();
        ISession GetCurrentSessionFor<TEntity>();
    }
}
