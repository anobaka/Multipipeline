using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace LazyMortal.Multipipeline
{
	public interface IPipeline
	{
		/// <summary>
		/// Pipeline's name
		/// </summary>
		string Name { get; set; }
		/// <summary>
		/// Whether a request belongs to this pipeline
		/// </summary>
		/// <param name="ctx"></param>
		/// <returns></returns>
		Task<bool> ResolveAsync(HttpContext ctx);
		/// <summary>
		/// <para>Configure this pipeline.</para>
		/// <para>e.g. Add some specific authentications for this pipeline</para>
		/// </summary>
		/// <param name="app"></param>
		/// <returns></returns>
		Task ConfigurePipeline(IApplicationBuilder app);
	}
}