using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Anobaka.Multipipeline
{
    public static class HttpContextExtensions
    {
	    private static string _httpContextItemKey;
		public static IPipeline GetPipeline(this HttpContext ctx)
	    {
		    if (string.IsNullOrEmpty(_httpContextItemKey))
		    {
			    _httpContextItemKey =
				    ctx.RequestServices.GetRequiredService<IOptions<MultipipelineOptions>>().Value.PipelineHttpContextItemKey;
		    }
		    object pipeline;
		    if (ctx.Items.TryGetValue(_httpContextItemKey, out pipeline))
		    {
			    return (IPipeline) pipeline;
		    }
		    return null;
	    }
    }
}
