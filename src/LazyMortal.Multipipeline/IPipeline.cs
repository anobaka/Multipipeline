using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace LazyMortal.Multipipeline
{
	public interface IPipeline
	{
		/// <summary>
		/// Pipeline's name, and it's recommended to be unique.
		/// </summary>
		string Name { get; set; }
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