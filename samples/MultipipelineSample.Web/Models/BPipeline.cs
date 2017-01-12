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
    public class BPipeline : DefaultPipeline
	{
		public override string Name { get; set; } = "B";
		public override Task<bool> ResolveAsync(HttpContext ctx)
		{
			return Task.FromResult(ctx.Request.Query.ContainsKey("b"));
		}

		public override Task ConfigurePipeline(IApplicationBuilder app)
		{
			app.UseSession(new SessionOptions { CookieName = nameof(BPipeline) });
			// others' middleware & configuration
			// eg.
			//		LoggingMiddleware
			//		AAuthenticationMiddleware
			return TaskCache.CompletedTask;
		}
	}
}
