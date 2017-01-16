using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LazyMortal.Multipipeline.DecisionTree;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;

namespace LazyMortal.Multipipeline
{

	public static class MultipipelineApplicationBuilderExtensions
	{
		/// <summary>
		/// use <see cref="MultipipelineOptions"/> as default pipeline options. To use custom options, try <see cref="AddMultipipeline{TOptions}"/>
		/// </summary>
		public static IApplicationBuilder AddMultipipeline(this IApplicationBuilder builder,
			Action<IApplicationBuilder> defaultConfiguration = null)
		{
			return builder.AddMultipipeline<MultipipelineOptions>(defaultConfiguration);
		}

		/// <summary>
		///
		/// </summary>
		/// <typeparam name="TOptions">Options model inheriting from <see cref="MultipipelineOptions"/>, is used for custom features</typeparam>
		/// <returns></returns>
		public static IApplicationBuilder AddMultipipeline<TOptions>(this IApplicationBuilder builder,
			Action<IApplicationBuilder> defaultConfiguration = null)
			where TOptions : MultipipelineOptions, new()
		{
			var options = builder.ApplicationServices.GetRequiredService<IOptions<TOptions>>();
			var decisionTree = builder.ApplicationServices.GetRequiredService<PipelineDecisionTree>();
			var pipelines = builder.ApplicationServices.GetRequiredService<PipelineCollectionAccessor>();

			builder.Use(
				t =>
					new MultipipelineMiddleware<TOptions>(t, builder,
							(ILoggerFactory) builder.ApplicationServices.GetService(typeof(ILoggerFactory)), options, decisionTree, pipelines,
							defaultConfiguration)
						.Invoke);
			return builder;
		}
	}
}