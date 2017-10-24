using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LazyMortal.Multipipeline
{
    public static class HttpContextExtensions
    {
        private static string _httpContextItemKey;

        /// <summary>
        /// Get current pipeline from <see cref="HttpContext.Items"/>. The default key is "Pipeline".
        /// </summary>
        /// <param name="ctx">aaaaa</param>
        /// <returns></returns>
        public static IPipeline GetPipeline(this HttpContext ctx)
        {
            if (string.IsNullOrEmpty(_httpContextItemKey))
            {
                _httpContextItemKey =
                    ctx.RequestServices.GetRequiredService<IOptions<MultipipelineOptions>>().Value
                        .PipelineHttpContextItemKey;
            }
            if (ctx.Items.TryGetValue(_httpContextItemKey, out var pipeline))
            {
                return (IPipeline) pipeline;
            }
            return null;
        }
    }
}