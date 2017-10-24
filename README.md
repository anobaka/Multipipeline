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

**1. Define your pipelines.**

```
public class DefaultPipeline : IPipeline
{
    public string Id { get; } = "Id-Default";
    public string ParentId { get; } = null;
    public virtual string Name { get; set; } = "Default";

    public virtual Task<bool> ResolveAsync(HttpContext ctx)
    {
        return Task.FromResult(true);
    }

    public virtual Task ConfigurePipeline(IApplicationBuilder app)
    {
        app.UseSession();
        return TaskCache.CompletedTask;
    }
}

public class APipeline : IPipeline
{
    public string Id { get; } = "Id-A";
    public string ParentId { get; } = "Id-Default";
    public string Name { get; set; } = "A";
	public Task<bool> ResolveAsync(HttpContext ctx)
	{
		return Task.FromResult(ctx.Request.Query.ContainsKey("a"));
	}

	public Task ConfigurePipeline(IApplicationBuilder app)
	{
		app.UseSession(new SessionOptions {CookieName = nameof(APipeline)});
		// others' middleware & configuration
		// eg.
		//		LoggingMiddleware
		//		AAuthenticationMiddleware
		return TaskCache.CompletedTask;
	}
}

    public class BPipeline : IPipeline
{
	public string Id { get; }
	public string ParentId { get; }
	public string Name { get; }

    public BPipeline(string id, string parentId, string name)
    {
        Id = id;
        ParentId = parentId;
        Name = name;
    }

	public Task<bool> ResolveAsync(HttpContext ctx)
	{
		return Task.FromResult(ctx.Request.Query.ContainsKey(Name));
	}

	public Task ConfigurePipeline(IApplicationBuilder app)
	{
		app.UseSession(new SessionOptions { CookieName = Id });
		// others' middleware & configuration
		// eg.
		//		LoggingMiddleware
		//		AAuthenticationMiddleware
		return TaskCache.CompletedTask;
	}
}
```

**2. Register your pipeline instances in Startup.cs.**

```
//...
public void ConfigureServices(IServiceCollection services)
{
    //other services
    services.AddMultipipeline(new List<IPipeline>
    {
        new DefaultPipeline(),
        new APipeline(),
        new BPipeline("Id-B1", "Id-A", "B1"),
        new BPipeline("Id-B2", "Id-Default", "B2")
    });
}

public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
{
    //other configuration and middlewares
    app.AddMultipipeline();
    //other configuration and middlewares
}

```

**3. Check the behaviors.**

+ Access `?a=` it will output the values of session of instance of APipeline.
+ Access `?b1=` it will output the values of session of the `b1` instance of BPipeline.
+ Access `?b2=` it will output the values of session of the `b2` instance of BPipeline.
+ Access others urls it will output the values of session of the instance of DefaultPipeline.

For further information, please see samples in repository.

## Roadmap

|Version|Release Date|Remark|
|:-----:|:-----:|:-----:|
|0.0.3|2017-03-27| - |
|0.1.0-beta|2017-10-20| - |

## Release Notes

### 0.1.0-beta
1. Redesigned this component.
It allows creating multiple pipeline instances by one type, 
and the relationships of them are made by `Id` and `ParentId` properties of `IPipeline`.
2. Upgraded project files to VS2017.
3. For compatible considering, some references(1.1.1) have been downgraded(1.1.0) which may used by projects with both **project.json** and **csproj**.

### 0.0.3
It will find all types inherited from `IPipeline` and initialize them by .Net Core DI Container.
The first pipeline(randomly) who resolves the current `HttpContext` will be the **current pipeline**,
And you can do something awesome with this pipeline.

## Contribution

Please start a discussion on the <a href="https://github.com/LazyMortal/Multipipeline/issues">repo issue tracker</a>.
