using Microsoft.AspNet.Identity;
using NHibernate.AspNet.Identity.Tests.Models;
using NUnit.Framework;
using System;
using System.Linq;
using System.Security.Claims;
using System.Transactions;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestMethod = NUnit.Framework.TestAttribute;

namespace NHibernate.AspNet.Identity.Tests
{
    [TestClass]
    public class RoleStoreTest
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
            _session.Dispose();
        }

        [TestMethod]
        public void WhenCreateRole()
        {
            var roleManager = new RoleManager<ApplicationRole>(new RoleStore<ApplicationRole>(_session));
            var role = new ApplicationRole() { Name = "RealRoleName" };

            using (var transaction = new TransactionScope())
            {
                var result = roleManager.CreateAsync(role).GetAwaiter().GetResult();
                transaction.Complete();
                Assert.AreEqual(0, result.Errors.Count());
            }

            var actual = _session.Query<ApplicationRole>().FirstOrDefault(x => x.Name == role.Name);

            Assert.IsNotNull(actual);
            Assert.AreEqual(role.Name, actual.Name);
        }

        [TestMethod]
        public void WhenDeleteRole()
        {
            var roleManager = new RoleManager<ApplicationRole>(new RoleStore<ApplicationRole>(_session));
            var role = new ApplicationRole() { Name = "RealRoleName" };

            using (var transaction = new TransactionScope())
            {
                var result = roleManager.CreateAsync(role).GetAwaiter().GetResult();
                transaction.Complete();
                Assert.AreEqual(0, result.Errors.Count());
            }

            using (var transaction = new TransactionScope())
            {
                var result = roleManager.DeleteAsync(role).GetAwaiter().GetResult();
                transaction.Complete();
                Assert.AreEqual(0, result.Errors.Count());
            }

            var actual = _session.Query<ApplicationRole>().FirstOrDefault(x => x.Name == role.Name);

            Assert.IsNull(actual);
        }

        [TestMethod]
        public void FindByName()
        {
            var roleManager = new RoleManager<ApplicationRole>(new RoleStore<ApplicationRole>(_session));
            roleManager.Create(new ApplicationRole() { Name = "test" });
            var x = roleManager.FindByName("test");
            Assert.IsNotNull(x);
        }

        [TestMethod]
        public void FindByNameIsCaseInsensitive()
        {
            var roleManager = new RoleManager<ApplicationRole>(new RoleStore<ApplicationRole>(_session));
            roleManager.Create(new ApplicationRole() { Name = "test" });
            var x = roleManager.FindByName("TeSt");
            Assert.IsNotNull(x);
        }

        [TestMethod]
        public void FindByNameNotExisting()
        {
            var roleManager = new RoleManager<ApplicationRole>(new RoleStore<ApplicationRole>(_session));
            var x = roleManager.FindByName("xxx");
            Assert.IsNull(x);
        }

        [TestMethod]
        public void FindById()
        {
            var roleManager = new RoleManager<ApplicationRole>(new RoleStore<ApplicationRole>(_session));
            var role = new ApplicationRole() { Name = "test" };
            roleManager.Create(role);
            var x = roleManager.FindById(role.Id);
            Assert.IsNotNull(x);
        }

        [TestMethod]
        public void FindByIdNotExisting()
        {
            var roleManager = new RoleManager<ApplicationRole>(new RoleStore<ApplicationRole>(_session));
            var x = roleManager.FindById(new Guid().ToString());
            Assert.IsNull(x);
        }
    }
}
