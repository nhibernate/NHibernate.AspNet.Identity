using System;
using System.IO;
using NHibernate.AspNet.Identity.Helpers;
using NHibernate.AspNet.Web.Models;
using NUnit.Framework;
using SpecsFor.Mvc;
using NHibernate.Mapping.ByCode;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using NHibernate.Context;

namespace NHibernate.AspNet.Web.Specs
{
	[SetUpFixture]
	public class AssemblyStartup
	{
		private SpecsForIntegrationHost _host;

		[SetUp]
		public void SetupTestRun()
		{
            HostStart();
            ConfigureDataBase();
		}

        private void HostStart()
        {
            var config = new SpecsForMvcConfig();
            config.UseIISExpress()
                .With(Project.Named("NHibernate.AspNet.Web"))
                .CleanupPublishedFiles()
                .ApplyWebConfigTransformForConfig("Debug");

            //TODO: The order of registration matters right now, but it shouldn't. 
            //config.RegisterArea<TasksAreaRegistration>();
            config.BuildRoutesUsing(r => RouteConfig.RegisterRoutes(r));

            //NOTE: You can use whatever browser you want.  For build servers, you can check an environment
            //		variable to determine which browser to use, enabling you to re-run the same suite of
            //		tests once for each browser. 
            //config.UseBrowser(BrowserDriver.InternetExplorer);
            //config.UseBrowser(BrowserDriver.Chrome);
            config.UseBrowser(BrowserDriver.Firefox);

            config.InterceptEmailMessagesOnPort(13565);

            //config.AuthenticateBeforeEachTestUsing<StandardAuthenticator>();

            _host = new SpecsForIntegrationHost(config);
            _host.Start();
        }

        private void ConfigureDataBase()
        {
            var root = AppDomain.CurrentDomain.BaseDirectory;
            AppDomain.CurrentDomain.SetData(
                "DataDirectory", Path.Combine(root, @"SpecsForMvc.TestSite\App_Data"));

            var internalTypes = new[] {
                typeof(ApplicationUser)
            };

            var mapping = MappingHelper.GetIdentityMappings(internalTypes);
            System.Diagnostics.Debug.WriteLine(mapping.AsString());

            var config = new Configuration().Configure();
            config.AddDeserializedMapping(mapping, null);
            BuildSchema(config);

            var factory = config.BuildSessionFactory();
            SessionResolver.RegisterFactoryToResolve(factory);
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


		[TearDown]
		public void TearDownTestRun()
		{
            foreach (var factory in SessionResolver.Current.GetAllFactories())
            {
                if (CurrentSessionContext.HasBind(factory))
                {
                    ISession session = CurrentSessionContext.Unbind(factory);

                    if (session.Transaction.IsActive)
                        session.Transaction.Rollback();

                    if (session != null && session.IsOpen)
                        session.Close();
                }
            }

			_host.Shutdown();
		}
	}
}