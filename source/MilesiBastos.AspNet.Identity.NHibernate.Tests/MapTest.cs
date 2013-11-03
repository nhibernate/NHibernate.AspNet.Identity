using FluentNHibernate.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate.Cfg;
using NHibernate.Mapping.ByCode;
using NHibernate.Tool.hbm2ddl;
using SharpArch.NHibernate;
using System;
using System.IO;

namespace MilesiBastos.AspNet.Identity.NHibernate.Tests
{
    [TestClass]
    public class MapTest
    {
        [TestMethod]
        public void CanCorrectlyMapIdentityUser()
        {
            var mapper = new ConventionModelMapper();

            mapper.AddMapping<IdentityUserMap>();
            mapper.AddMapping<IdentityRoleMap>();
            mapper.AddMapping<IdentityUserClaimMap>();
            mapper.AddMapping<IdentityUserLoginMap>();

            var mapping = mapper.CompileMappingForAllExplicitlyAddedEntities();

            var configuration = NHibernateSession.Init(
                new SimpleSessionStorage(), mapping, "sqlite-nhibernate-config.xml");

            BuildSchema(configuration);

            var session = NHibernateSession.Current;

            new PersistenceSpecification<IdentityUser>(session)
                .CheckProperty(c => c.UserName, "John")
                .VerifyTheMappings();

            NHibernateSession.Reset();
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
