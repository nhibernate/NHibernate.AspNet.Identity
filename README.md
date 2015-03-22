[![Build status](https://ci.appveyor.com/api/projects/status/m3in48hf0846qtot?svg=true)](https://ci.appveyor.com/project/milesibastos/nhibernate-aspnet-identity)
[![Stories in Ready](https://badge.waffle.io/milesibastos/nhibernate.aspnet.identity.png?label=ready&title=Ready)](https://waffle.io/milesibastos/nhibernate.aspnet.identity)

NHibernate.AspNet.Identity
=======================

ASP.NET Identity provider that users NHibernate for storage

## About this ##

[![About this](http://milesibastos.github.io/NHibernate.AspNet.Identity/images/about.png)](http://youtu.be/pb4QgjXkT7E)

## Purpose ##

ASP.NET MVC 5 shipped with a new Identity system (in the Microsoft.AspNet.Identity.Core package) in order to support both local login and remote logins via OpenID/OAuth, but only ships with an
Entity Framework provider (Microsoft.AspNet.Identity.EntityFramework).

## Features ##
* Drop-in replacement ASP.NET Identity with NHibernate as the backing store.
* Based on same schema requirede by EntityFramework for compatibility model
* Contains the same IdentityUser class used by the EntityFramework provider in the MVC 5 project template.
* Supports additional profile properties on your application's user model.
* Provides UserStore<TUser> implementation that implements the same interfaces as the EntityFramework version:
    * IUserStore<TUser>
    * IUserLoginStore<TUser>
    * IUserRoleStore<TUser>
    * IUserClaimStore<TUser>
    * IUserPasswordStore<TUser>
    * IUserSecurityStampStore<TUser>

## Quick-start guide ##
These instructions assume you know how to set up NHibernate within an MVC application.

1. Create a new ASP.NET MVC 5 project, choosing the Individual User Accounts authentication type.
2. Remove the Entity Framework packages and replace with NHibernate Identity:

```PowerShell
Uninstall-Package Microsoft.AspNet.Identity.EntityFramework
Uninstall-Package EntityFramework
Install-Package NHibernate.AspNet.Identity
```
    
3. In ~/Models/IdentityModels.cs:
    * Remove the namespace: Microsoft.AspNet.Identity.EntityFramework
    * Add the namespace: NHibernate.AspNet.Identity
	* Remove the ApplicationDbContext class completely.
4. In ~/Controllers/AccountController.cs
    * Remove the namespace: Microsoft.AspNet.Identity.EntityFramework
    * Add the relevant ISession implementation that will be used by default.  This could be from a DI implementation.
	Note: This isn't mandatory, if you are using a framework that will inject the dependency, you shouldn't need the parameterless constructor.

5. Setup configuration code

NHibernate
```C#

	// this assumes you are using the default Identity model of "ApplicationUser"
	var myEntities = new [] {
		typeof(ApplicationUser)
	};
	
    var configuration = new Configuration();
    configuration.Configure("sqlite-nhibernate-config.xml");
    configuration.AddDeserializedMapping(MappingHelper.GetIdentityMappings(myEntities), null);

    var factory = configuration.BuildSessionFactory();
    var session = factory.OpenSession();

    var userManager = new UserManager<ApplicationUser>(
        new UserStore<ApplicationUser>(session);
```

FluentNHibernate
```C#
	// this assumes you are using the default Identity model of "ApplicationUser"
	var myEntities = new [] {
		typeof(ApplicationUser)
	};
	
	var configuration = Fluently.Configure()
	   .Database(/*.....*/)
	   .ExposeConfiguration(cfg => {
	       cfg.AddDeserializedMapping(MappingHelper.GetIdentityMappings(myEntities), null);
		});
	
    var factory = configuration.BuildSessionFactory();
    var session = factory.OpenSession();

    var userManager = new UserManager<ApplicationUser>(
        new UserStore<ApplicationUser>(session);

```


## Thanks To ##

Special thanks to [David Boike](https://github.com/DavidBoike) whos [RavenDB AspNet Identity](https://github.com/ILMServices/RavenDB.AspNet.Identity) project gave me the base for jumpstarting the NHibernate provider