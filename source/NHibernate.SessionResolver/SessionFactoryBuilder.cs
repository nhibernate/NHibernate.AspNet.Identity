using System.Collections.Generic;
using NHibernate.Cfg;
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
            foreach (var configuration in _configurations) {
                var factory = configuration.BuildSessionFactory();
                SessionResolver.RegisterFactoryToResolve(factory);
            }

            app.Use<SessionMiddleware>(SessionResolver.Current);
        }
    }
}
