using Microsoft.AspNet.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate.Linq;
using System.Linq;

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
    }
}
