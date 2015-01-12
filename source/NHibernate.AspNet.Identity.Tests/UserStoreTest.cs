using System.Linq;
using System.Transactions;
using Microsoft.AspNet.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate.AspNet.Identity.Tests.Models;
using NHibernate.Linq;

namespace NHibernate.AspNet.Identity.Tests
{
    [TestClass]
    public class UserStoreTest
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
        public void WhenHaveNoUser()
        {
            var login = new UserLoginInfo("ProviderTest", "ProviderKey");
            var store = new UserStore<IdentityUser>(_session);
            var user = store.FindAsync(login).Result;

            Assert.IsNull(user);
        }

        [TestMethod]
        public void WhenAddLoginAsync()
        {
            var user = new IdentityUser("Lukz");
            var login = new UserLoginInfo("ProviderTest02", "ProviderKey02");
            var store = new UserStore<IdentityUser>(_session);
            var result = store.AddLoginAsync(user, login);

            var actual = _session.Query<IdentityUser>().FirstOrDefault(x => x.UserName == user.UserName);
            var userStored = store.FindAsync(login).Result;

            Assert.IsNull(result.Exception);
            Assert.IsNotNull(actual);
            Assert.AreEqual(user.UserName, actual.UserName);
            Assert.AreEqual(user.UserName, userStored.UserName);
        }

        [TestMethod]
        public void WhenRemoveLoginAsync()
        {
            var user = new IdentityUser("Lukz 03");
            var login = new UserLoginInfo("ProviderTest03", "ProviderKey03");
            var store = new UserStore<IdentityUser>(_session);
            store.AddLoginAsync(user, login);

            Assert.IsTrue(user.Logins.Any());

            var result = store.RemoveLoginAsync(user, login);

            var actual = _session.Query<IdentityUser>().FirstOrDefault(x => x.UserName == user.UserName);
            Assert.IsNull(result.Exception);
            Assert.IsFalse(actual.Logins.Any());
        }

        [TestMethod]
        public void WhenCeateUserAsync()
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_session));
            var user = new ApplicationUser() { UserName = "RealUserName" };

            using (var transaction = new TransactionScope())
            {
                var result = userManager.CreateAsync(user, "RealPassword");
                transaction.Complete();
                Assert.IsNull(result.Exception);
            }

            var actual = _session.Query<ApplicationUser>().FirstOrDefault(x => x.UserName == user.UserName);

            Assert.IsNotNull(actual);
            Assert.AreEqual(user.UserName, actual.UserName);
        }

        [TestMethod]
        public void GivenHaveRoles_WhenDeleteUser_ThenDeletingCausesNoCascade()
        {
            var user = new IdentityUser("Lukz 04");
            var role = new IdentityRole("ADM");
            var store = new UserStore<IdentityUser>(_session);
            var roleStore = new RoleStore<IdentityRole>(_session);

            roleStore.CreateAsync(role);
            store.CreateAsync(user);
            store.AddToRoleAsync(user, "ADM");

            Assert.IsTrue(_session.Query<IdentityRole>().Any(x => x.Name == "ADM"));
            Assert.IsTrue(_session.Query<IdentityUser>().Any(x => x.UserName == "Lukz 04"));

            var result = store.DeleteAsync(user);

            Assert.IsNull(result.Exception);
            Assert.IsFalse(_session.Query<IdentityUser>().Any(x => x.UserName == "Lukz 04"));
            Assert.IsTrue(_session.Query<IdentityRole>().Any(x => x.Name == "ADM"));
        }

        [TestMethod]
        public void WhenRemoveUserFromRole_ThenDoNotDeleteRole_BugFix()
        {
            var user = new IdentityUser("Lukz 05");
            var role = new IdentityRole("ADM05");
            var store = new UserStore<IdentityUser>(_session);
            var roleStore = new RoleStore<IdentityRole>(_session);

            roleStore.CreateAsync(role);
            store.CreateAsync(user);
            store.AddToRoleAsync(user, "ADM05");

            Assert.IsTrue(_session.Query<IdentityRole>().Any(x => x.Name == "ADM05"));
            Assert.IsTrue(_session.Query<IdentityUser>().Any(x => x.UserName == "Lukz 05"));
            Assert.IsTrue(store.IsInRoleAsync(user, "ADM05").Result);

            var result = store.RemoveFromRoleAsync(user, "ADM05");

            Assert.IsNull(result.Exception);
            Assert.IsFalse(store.IsInRoleAsync(user, "ADM05").Result);
            Assert.IsTrue(_session.Query<IdentityUser>().Any(x => x.UserName == "Lukz 05"));
            Assert.IsTrue(_session.Query<IdentityRole>().Any(x => x.Name == "ADM05"));
        }

    }
}
