using System;
using System.IO;
using System.Linq;
using NHibernate.AspNet.Identity;
using NHibernate.AspNet.Web.Models;
using NHibernate.Cfg;
using NHibernate.Mapping.ByCode;
using NHibernate.Tool.hbm2ddl;
using SharpArch.Domain.DomainModel;
using SharpArch.NHibernate;

namespace NHibernate.AspNet.Web
{
    public class DataConfig
    {
        public static void Configure(ISessionStorage storage)
        {
            var baseEntityToIgnore = new[] { 
                typeof(NHibernate.AspNet.Identity.DomainModel.EntityWithTypedId<int>), 
                typeof(NHibernate.AspNet.Identity.DomainModel.EntityWithTypedId<string>), 
            };

            var allEntities = new[] { 
                typeof(IdentityUser), 
                typeof(ApplicationUser), 
                typeof(IdentityRole), 
                typeof(IdentityUserLogin), 
                typeof(IdentityUserClaim), 
            };

            var mapper = new ConventionModelMapper();
            DefineBaseClass(mapper, baseEntityToIgnore);
            mapper.IsComponent((type, declared) => typeof(NHibernate.AspNet.Identity.DomainModel.ValueObject).IsAssignableFrom(type));

            mapper.AddMapping<IdentityUserMap>();
            mapper.AddMapping<IdentityRoleMap>();
            mapper.AddMapping<IdentityUserClaimMap>();

            var mapping = mapper.CompileMappingFor(allEntities);
            System.Diagnostics.Debug.WriteLine(mapping.AsString());

            var configuration = NHibernateSession.Init(storage, mapping);
            BuildSchema(configuration);
        }

        private static void DefineBaseClass(ConventionModelMapper mapper, System.Type[] baseEntityToIgnore)
        {
            if (baseEntityToIgnore == null) return;
            mapper.IsEntity((type, declared) =>
                baseEntityToIgnore.Any(x => x.IsAssignableFrom(type)) &&
                !baseEntityToIgnore.Any(x => x == type) &&
                !type.IsInterface);
            mapper.IsRootEntity((type, declared) => baseEntityToIgnore.Any(x => x == type.BaseType));
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