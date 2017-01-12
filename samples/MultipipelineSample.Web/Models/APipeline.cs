using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anobaka.Multipipeline;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Internal;

namespace MultipipelineSample.Web.Models
{
    public class APipeline : DefaultPipeline
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
}
