using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentNHibernate.Testing;
using FluentNHibernate.Cfg.Db;
using SharpArch.NHibernate;
using NHibernate.Mapping.ByCode;
using SharpArch.Domain.DomainModel;
using NHibernate.Cfg;
using NHibernate;
using System.IO;
using NHibernate.Tool.hbm2ddl;

namespace MilesiBastos.AspNet.Identity.NHibernate.Tests
{
    [TestClass]
    public class MapTest
    {
        [TestMethod]
        public void CanCorrectlyMapIdentityUser()
        {
            //var persistenceConfigurer = SQLiteConfiguration.Standard.ConnectionString(c => c.Is("Data Source=:memory:;Version=3;New=True;"));
            //var mappingAssemblies = new string[] { };
            //var configuration = NHibernateSession.Init(
            //    new SimpleSessionStorage(), mappingAssemblies, null, null, null, null, persistenceConfigurer);

            var mapper = new ModelMapper();
            //var mapper = new ConventionModelMapper();
            //var baseEntityType = typeof(EntityWithTypedId<>);
            //mapper.IsEntity((t, declared) => baseEntityType.IsAssignableFrom(t) && baseEntityType != t && !t.IsInterface);
            //mapper.IsRootEntity((t, declared) => baseEntityType.Equals(t.BaseType));
            //mapper.Class<BaseEntity>(
            //    map =>
            //    {
            //        map.Id(x => x.Id, idmap => { });
            //        map.Version(x => x.Version, vm => { });
            //    });

            mapper.BeforeMapManyToOne += (insp, prop, map) => map.Column(prop.LocalMember.GetPropertyOrFieldType().Name + "Id");
            mapper.BeforeMapManyToOne += (insp, prop, map) => map.Cascade(Cascade.Persist);
            mapper.BeforeMapBag += (insp, prop, map) => map.Key(km => km.Column(prop.GetContainerEntity(insp).Name + "Id"));
            mapper.BeforeMapBag += (insp, prop, map) => map.Cascade(Cascade.All);

            //mapper.Class<IdentityUser>(
            //    map =>
            //    {
            //        map.Table("AspNetUsers");
            //        map.Id(x => x.Id, m => m.Generator(new UUIDHexCombGeneratorDef("D")));
            //        map.Property(x => x.UserName);
            //        //map.Bag(x => x.Roles, m => m.Table("AspNetUserRoles"));
            //    });

            mapper.AddMapping<IdentityUserMap>();
            mapper.AddMapping<IdentityRoleMap>();
            mapper.AddMapping<IdentityUserClaimMap>();
            mapper.AddMapping<IdentityUserLoginMap>();

            //var mapping = mapper.CompileMappingFor(new[] { 
            //    typeof(IdentityUser), 
            //    typeof(IdentityRole), 
            //    typeof(IdentityUserClaim), 
            //    typeof(IdentityUserLogin)
            //});

            var mapping = mapper.CompileMappingForAllExplicitlyAddedEntities();

            var configuration = new Configuration();
            configuration.Configure("sqlite-nhibernate-config.xml");
            configuration.AddDeserializedMapping(mapping, null);

            var factory = configuration.BuildSessionFactory();
            var session = factory.OpenSession();
            BuildSchema(configuration);

            //new PersistenceSpecification<IdentityUser>(session)
            //    .CheckProperty(c => c.UserName, "John")
            //    .VerifyTheMappings();
        }

        private static void BuildSchema(Configuration config)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"App_Data\sql\etl-schema.sql");
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
