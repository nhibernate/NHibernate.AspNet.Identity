using NHibernate.AspNet.Web.Controllers;
using NHibernate.AspNet.Web.Models;
using SpecsFor.Mvc;
using SpecsFor.Mvc.Authentication;

namespace NHibernate.AspNet.Web.Specs
{
	public class StandardAuthenticator : IHandleAuthentication
	{
		public void Authenticate(MvcWebApp mvcWebApp)
		{
            mvcWebApp.NavigateTo<AccountController>(c => c.Login(string.Empty));

            mvcWebApp.FindFormFor<LoginViewModel>()
				.Field(m => m.UserName).SetValueTo("real@user.com")
				.Field(m => m.Password).SetValueTo("RealPassword")
				.Submit();
		}
	}
}