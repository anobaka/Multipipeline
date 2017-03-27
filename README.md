# Multipipeline

_referred and modified from https://github.com/saaskit/saaskit_

---

With `Multipipeline` we can create multiple request pipelines which can have different behaviors on same business code.

## Common Usages
+ Creating multiple UI skins applications.
+ Applying different authentication and authorization policies for third parties.
+ ...


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
public class SomePipeline : DefaultPipeline
{
    public override string Name { get; set; } = "A";
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

## Contribution

Please start a discussion on the <a href="https://github.com/LazyMortal/Multipipeline/issues">repo issue tracker</a>.
