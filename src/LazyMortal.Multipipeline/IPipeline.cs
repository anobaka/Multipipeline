using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace LazyMortal.Multipipeline
{
    public interface IPipeline
    {
        /// <summary>
        /// Id of current pipeline, and it muse be unique.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Id of parent pipeline.
        /// </summary>
        string ParentId { get; }
        /// <summary>
        /// Name of pipeline.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Whether a context belongs to this pipeline
        /// </summary>
        /// <param name="ctx">Current context.</param>
        /// <returns></returns>
        Task<bool> ResolveAsync(HttpContext ctx);

        /// <summary>
        /// <para>Configure this pipeline.</para>
        /// <para>e.g. Add some specific authentications for this pipeline</para>
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <returns></returns>
        Task ConfigurePipeline(IApplicationBuilder app);
    }
}