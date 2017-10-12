using System;
using System.IO;
using NUnit.Framework;
using SharpArch.NHibernate;
using SpecsFor.Mvc;

namespace NHibernate.AspNet.Web.Specs
{
	[SetUpFixture]
	public class AssemblyStartup
	{
		private SpecsForIntegrationHost _host;

		[OneTimeSetUp]
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

            DataConfig.Configure(new SimpleSessionStorage());
        }

		[OneTimeTearDown]
		public void TearDownTestRun()
		{
			_host.Shutdown();
		}
	}
}
