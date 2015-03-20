using FluentNHibernate.Testing;
using NHibernate.AspNet.Identity.Tests.Models;
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;

namespace NHibernate.AspNet.Identity.Tests
{
    [TestClass]
    public class MapTest
    {
        ISession _session;

        [TestInitialize]
        public void Initialize()
        {
            var factory = SessionFactoryProvider.Instance.SessionFactory;
            _session = factory.OpenSession();
            SessionFactoryProvider.Instance.BuildSchema();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _session.Close();
        }

        [TestMethod]
        public void CanCorrectlyMapFoo()
        {
            new PersistenceSpecification<Foo>(_session)
                .CheckProperty(c => c.String, "Foo")
                .CheckReference(r => r.User, new ApplicationUser { UserName = "FooUser" })
                .VerifyTheMappings();
        }

        [TestMethod]
        public void CanCorrectlyMapIdentityUser()
        {
            new PersistenceSpecification<IdentityUser>(_session)
                .CheckProperty(c => c.UserName, "Lukz")
                .VerifyTheMappings();
        }

        [TestMethod]
        public void CanCorrectlyMapApplicationUser()
        {
            new PersistenceSpecification<ApplicationUser>(_session)
                .CheckProperty(c => c.UserName, "Lukz")
                .VerifyTheMappings();
        }

        [TestMethod]
        public void CanCorrectlyMapCascadeLogins()
        {
            new PersistenceSpecification<IdentityUser>(_session)
                .CheckProperty(c => c.UserName, "Letícia")
                .CheckComponentList(c => c.Logins, new[] { new IdentityUserLogin { LoginProvider = "Provider", ProviderKey = "Key" } })
                //.CheckList(l => l.Logins, new[] { new IdentityUserLogin { LoginProvider = "Provider", ProviderKey = "Key" } })
                //.CheckList(l => l.Logins, new[] { new IdentityUserLogin { LoginProvider = "Provider", ProviderKey = "Key" } }, (user, login) => user.Logins.Add(login))
                .VerifyTheMappings();
        }
    }
}
