using System.Collections.Generic;
using System.Linq;
using NHibernate.Cfg;
using NHibernate.Context;
using NHibernate.Engine;
using NHibernate.Owin;
using Owin;

namespace NHibernate
{
    public class SessionFactoryBuilder
    {
        private readonly ISet<Configuration> _configurations;

        public SessionFactoryBuilder()
        {
            _configurations = new HashSet<Configuration>();
        }

        public void RegisterToBuild(Configuration configuration)
        {
            _configurations.Add(configuration);
        }

        public void Build(IAppBuilder app)
        {
            var resolver = new DefaultSessionResolver();
            foreach (var configuration in _configurations) {
                var factory = configuration.BuildSessionFactory();
                resolver.RegisterFactoryToResolve(factory);
            }

            app.Use<SessionMiddleware>(resolver);
            SessionResolver.SetResolver(resolver);
        }

        private class DefaultSessionResolver : ISessionResolver
        {
            private static readonly ISet<ISessionFactory> _factories;

            static DefaultSessionResolver()
            {
                _factories = new HashSet<ISessionFactory>();
            }

            internal void RegisterFactoryToResolve(ISessionFactory factory)
            {
                _factories.Add(factory);
            }

            public IEnumerable<ISessionFactory> GetAllFactories()
            {
                return _factories;
            }

            public ISessionFactory GetFactoryFor<TEntity>()
            {
                var type = typeof(TEntity);
                var factory = _factories.SingleOrDefault(x => ((ISessionFactoryImplementor)x).TryGetEntityPersister(type.FullName) != null);
                return factory;
            }

            public ISession GetCurrentSessionFor<TEntity>()
            {
                var factory = GetFactoryFor<TEntity>();
                if (!CurrentSessionContext.HasBind(factory))
                    CurrentSessionContext.Bind(factory.OpenSession());

                return factory.GetCurrentSession();
            }
        }
    }
}
