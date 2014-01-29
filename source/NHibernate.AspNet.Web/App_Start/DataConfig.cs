using System;
using System.IO;
using System.Web;
using NHibernate.AspNet.Identity;
using NHibernate.Cfg;
using NHibernate.Mapping.ByCode;
using NHibernate.Tool.hbm2ddl;
using SharpArch.Domain.DomainModel;
using SharpArch.NHibernate;
using SharpArch.NHibernate.Web.Mvc;

namespace NHibernate.AspNet.Web
{
    public class DataConfig
    {
        public static void Configure(HttpApplication app)
        {
            var baseEntityToIgnore = new[] { 
                typeof(EntityWithTypedId<int>), 
                typeof(EntityWithTypedId<string>), 
            };

            var allEntities = new[] { 
                typeof(IdentityUser), 
                typeof(IdentityRole), 
                typeof(IdentityUserLogin), 
                typeof(IdentityUserClaim), 
            };

            var mapper = new ModelMapper();

            mapper.AddMapping<IdentityUserMap>();
            mapper.AddMapping<IdentityRoleMap>();
            mapper.AddMapping<IdentityUserClaimMap>();

            var mapping = mapper.CompileMappingFor(allEntities);
            System.Diagnostics.Debug.WriteLine(mapping.AsString());

            var configuration = NHibernateSession.Init(new WebSessionStorage(app), mapping);
            BuildSchema(configuration);
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