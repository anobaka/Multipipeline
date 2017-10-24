using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LazyMortal.Multipipeline;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Internal;

namespace MultipipelineSample.Web.Models
{
    public class APipeline : IPipeline
    {
        public string Id { get; } = "Id-A";
        public string ParentId { get; } = "Id-Default";
        public string Name { get; } = "A";
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
}
