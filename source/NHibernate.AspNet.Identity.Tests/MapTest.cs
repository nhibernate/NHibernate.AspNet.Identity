using FluentNHibernate.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate.Cfg;
using NHibernate.Mapping.ByCode;
using NHibernate.Tool.hbm2ddl;
using System;
using System.IO;

namespace NHibernate.AspNet.Identity.Tests
{
    [TestClass]
    public class MapTest
    {
        [TestMethod]
        public void CanCorrectlyMapIdentityUser()
        {
            var mapper = new ModelMapper();
            mapper.AddMapping<IdentityUserMap>();
            mapper.AddMapping<IdentityRoleMap>();
            mapper.AddMapping<IdentityUserClaimMap>();
            mapper.AddMapping<IdentityUserLoginMap>();

            var mapping = mapper.CompileMappingForAllExplicitlyAddedEntities();

            var configuration = new Configuration();
            configuration.Configure("sqlite-nhibernate-config.xml");
            configuration.AddDeserializedMapping(mapping, null);

            var factory = configuration.BuildSessionFactory();
            var session = factory.OpenSession();
            BuildSchema(configuration);

            new PersistenceSpecification<IdentityUser>(session)
                .CheckProperty(c => c.UserName, "John")
                .VerifyTheMappings();
        }

        private static void BuildSchema(Configuration config)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"App_Data\sql\schema.sql");
            if (!Directory.Exists(Path.GetDirectoryName(path)))
                Directory.CreateDirectory(Path.GetDirectoryName(path));

            // this NHibernate tool takes a configuration (with mapping info in)
            // and exports a database schema from it
            new SchemaExport(config)
                .SetOutputFile(path)
                .Create(true, true /* DROP AND CREATE SCHEMA */);
        }
    }
}
