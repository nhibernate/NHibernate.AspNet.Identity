using System.Transactions;
using Microsoft.AspNet.Identity;
using NHibernate.AspNet.Identity;
using NHibernate.AspNet.Web.Controllers;
using NHibernate.AspNet.Web.Models;
using NUnit.Framework;
using SharpArch.NHibernate;
using Should;
using SpecsFor;
using SpecsFor.Mvc;
using SpecsFor.Mvc.Helpers;

namespace NHibernate.AspNet.Web.Specs
{
	public class LoginSpecs
	{
		public class when_logging_in_with_an_invalid_username_and_password : SpecsFor<MvcWebApp>
		{
			protected override void Given()
			{
				//Make sure we're already logged out
				SUT.NavigateTo<AccountController>(c => c.LogOff());

                SUT.NavigateTo<AccountController>(c => c.Login("/Home/About"));
			}

			protected override void When()
			{
                SUT.FindFormFor<LoginViewModel>()
					.Field(m => m.UserName).SetValueTo("bad@user.com")
					.Field(m => m.Password).SetValueTo("BadPass")
					.Submit();
			}

			[Test]
			public void then_it_should_redisplay_the_page()
			{
                SUT.Route.ShouldMapTo<AccountController>(c => c.Login("%2FHome%2FAbout"));
			}

			[Test]
			public void then_it_should_contain_a_validation_error()
			{
                SUT.ValidationSummary.Text.ShouldContain("Invalid username or password.");
			}
		}

		public class when_logging_in_with_valid_credentials : SpecsFor<MvcWebApp>
		{
			protected override void Given()
			{
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(NHibernateSession.Current));
                using (var transaction = new TransactionScope())
                {
                    var user = new ApplicationUser() { UserName = "RealUserName" };
                    var result = userManager.CreateAsync(user, "RealPassword");
                    
                    transaction.Complete();
                    result.Exception.ShouldBeNull();
                }

                SUT.NavigateTo<AccountController>(c => c.Login("/Home/About"));
			}

			protected override void When()
			{
                SUT.FindFormFor<LoginViewModel>()
                    .Field(m => m.UserName).SetValueTo("RealUserName")
					.Field(m => m.Password).SetValueTo("RealPassword")
					.Submit();
			}

			[Test]
			public void then_it_redirects_to_the_about_page()
			{
				SUT.Route.ShouldMapTo<HomeController>(c => c.About());
			}
		}
	}
}