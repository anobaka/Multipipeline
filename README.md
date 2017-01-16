# Multipipeline

_referred and modified from https://github.com/saaskit/saaskit_

---

With `Multipipeline` we can create multiple request pipelines which can have different behaviors on same business code.

## Common Usages
+ Creating multiple UI skins applications.
+ Applying different authentication policies and authorization policies for third parties.
+ ...


## Get Started
+ Startup.cs

```
//...
public void ConfigureServices(IServiceCollection services)
{
    //other services
    services.AddMultipipeline(t =>
    {
        t.Pipelines = new List<IPipeline>
        {
            new DefaultPipeline(),
            new APipeline(),
            new BPipeline()
            //other pipelines
        };
    });
}

public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
{
    //other configuration and middlewares
    app.AddMultipipeline();
    //other configuration and middlewares
}

```

For further information, please see samples in repository.

## Contribution

Please start a discussion on the <a href="https://github.com/LazyMortal/Multipipeline/issues">repo issue tracker</a>.