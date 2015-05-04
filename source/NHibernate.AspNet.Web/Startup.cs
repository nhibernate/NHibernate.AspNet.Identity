using System;
using System.IO;
using Microsoft.Owin;
using NHibernate.AspNet.Identity.Helpers;
using NHibernate.AspNet.Web.Models;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using Owin;

[assembly: OwinStartupAttribute(typeof(NHibernate.AspNet.Web.Startup))]
namespace NHibernate.AspNet.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            var internalTypes = new[] {
                typeof(ApplicationUser)
            };

            var mapping = MappingHelper.GetIdentityMappings(internalTypes);

            var config = new Configuration().Configure();
            config.AddDeserializedMapping(mapping, null);
            BuildSchema(config);

            var builder = new SessionFactoryBuilder();
            builder.RegisterToBuild(config);
            // Register a second config for multi data bases
            // builder.RegisterToBuild(secondconfig); 
            builder.Build(app);
        }

        private static void BuildSchema(Configuration config)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.GetData("DataDirectory").ToString(), @"schema.sql");

            // this NHibernate tool takes a configuration (with mapping info in)
            // and exports a database schema from it
            new SchemaExport(config)
                .SetOutputFile(path)
                .Create(true, true /* DROP AND CREATE SCHEMA */);
        }
    }
}
