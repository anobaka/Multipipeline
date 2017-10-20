# Multipipeline

_referred and modified from https://github.com/saaskit/saaskit_

---

With `Multipipeline` we can create multiple request pipelines which can have different behaviors on same business code.

## Common Usages
+ Creating multiple UI skins applications.
+ Applying different authentication and authorization policies for third parties.
+ ...

## Installation
+ <a href="https://www.nuget.org/packages/LazyMortal.Multipipeline/">Nuget</a>

## Get Started
+ Startup.cs

```
//...
public void ConfigureServices(IServiceCollection services)
{
    //other services
    services.AddMultipipeline();
}

public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
{
    //other configuration and middlewares
    app.AddMultipipeline();
    //other configuration and middlewares
}

```
+ SomePipeline.cs
```
public class SomePipeline
{
    public override string Name { get; set; } = "A";
    public override string Parent { get; set; } = "Default";
    public override Task<bool> ResolveAsync(HttpContext ctx)
    {
        return Task.FromResult(ctx.Request.Query.ContainsKey("a"));
    }

    public override Task ConfigurePipeline(IApplicationBuilder app)
    {
        app.UseSession(new SessionOptions {CookieName = nameof(APipeline)});
        // others' middleware & configuration
        // eg.
        //		LoggingMiddleware
        //		AAuthenticationMiddleware
        return TaskCache.CompletedTask;
    }
}
```

For further information, please see samples in repository.

## Release Notes

### 0.1.0
Redesigned this component.
It allows creating multiple pipeline instances by one type, 
and the relationships of them are made by `Id` and `ParentId` properties of `IPipeline`.

### 0.0.3
It will find all types inherited from `IPipeline` and initialize them by .Net Core DI Container.
The first pipeline(randomly) who resolves the current `HttpContext` will be the **current pipeline**,
And you can do something awesome with this pipeline.

## Contribution

Please start a discussion on the <a href="https://github.com/LazyMortal/Multipipeline/issues">repo issue tracker</a>.
