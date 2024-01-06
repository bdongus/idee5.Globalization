#  idee5.Globalization
Database driven tools for .NET localization. But different.
## Why another localizazion library?
The existing solutions are well suited for tailormade products or products without any modifcations for customers or groups of customers.

Every industry has its own parlance. The same thing has a different name and your customers might not understand you or your product.

Maybe you need to create custom versions for your customers industries. Keeping text in your UI in sync with all the versions can quickly create a lot of overhead.

Sometimes customers addiotionally have their own terminology. The mess is even growing.

This is why we created our own implementation of a database driven localization library.
## What makes this library differnt?
This library supports parlances for industries and customers. Every resource can have:
- a neutral version.
- an industry version.
- a customer version. 

All hidden behind the standard .NET localization. There is no need to make big changes in your product. Only a little bit of configuration.

## Getting started
There are three nuget packages available.
1. **idee5.Globalization**. The minimum you need.
2. **dee5.Globalization.EFCore**. An EF Core based implementation of the database layer.
3. **idee5.Globalization.WebApi**. ASP.NET Core integration. Including api controllers for your own application.

The code below assumes you are using the .NET DI container.
### Configure your Industry and customer
Using the Options pattern you are able to set the industry and/or customer. Both being `null` would be the neutral/standard version.
```csharp
services.Configure<ExtendedLocalizationOptions>(o => {
    o.Customer = null;
    o.Industry = null;
});
```
In production you should use another configuration provider. E.g. appsettings.json
### Register the EF Core module
Just replace the default `StringLocalizerFactory`.
```csharp
// add the EF Core localization implementation
services.AddEFCoreLocalization(o => o.UseSqlite(new SimpleDB3ConnectionStringProvider().GetConnectionString("idee5.Resources.db3")));
```
### Integrate the localization controllers
By adding the controllers you can use web api calls to read and mange your resources.
```csharp
// add the localization controllers
services.AddMvc().AddLocalizationControllers();
// configure the authorization policy
services.AddAuthorizationBuilder()
    .AddPolicy("CommandPolicy", p => p.RequireAssertion(_ => true))
    .AddPolicy("QueryPolicy", p => p.RequireAssertion(_ => true));
```
Protecting the localization controllers from unauthorized access can be configured with the `QueryPolicy` and the `CommandPolicy`.
### Managing the resources
For desktop applications we recommend using the commands and queries from **idee5.Globalization** directly.

For web applications we recommend the integration of the localization controllers from **idee5.Globalization.WebApi** into your application.

