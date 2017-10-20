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
}