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
}
