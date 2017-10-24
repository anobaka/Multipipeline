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
		/// Use <see cref="MultipipelineOptions"/> as default pipeline options. To use custom options, try <see cref="AddMultipipeline{TOptions}"/>
		/// </summary>
		/// <param name="builder"></param>
		/// <param name="defaultPipelineConfiguration">You can set a default behavior than other pipelines, but it's not a truly pipeline.</param>
		/// <returns></returns>
		public static IApplicationBuilder AddMultipipeline(this IApplicationBuilder builder,
			Action<IApplicationBuilder> defaultPipelineConfiguration = null)
		{
			return builder.AddMultipipeline<MultipipelineOptions>(defaultPipelineConfiguration);
		}

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TOptions">Options model inheriting from <see cref="MultipipelineOptions"/>, is used for custom features</typeparam>
        /// <param name="builder"></param>
        /// <param name="defaultPipelineConfiguration">You can set a default behavior than other pipelines, but it's not a truly pipeline.</param>
        /// <returns></returns>
        public static IApplicationBuilder AddMultipipeline<TOptions>(this IApplicationBuilder builder,
			Action<IApplicationBuilder> defaultPipelineConfiguration = null)
			where TOptions : MultipipelineOptions, new()
		{
			var options = builder.ApplicationServices.GetRequiredService<IOptions<TOptions>>();
			var decisionTree = builder.ApplicationServices.GetRequiredService<PipelineDecisionTree>();
			var pipelines = builder.ApplicationServices.GetRequiredService<PipelineCollectionAccessor>();

			builder.Use(
				t =>
					new MultipipelineMiddleware<TOptions>(t, builder,
							(ILoggerFactory) builder.ApplicationServices.GetService(typeof(ILoggerFactory)), options, decisionTree, pipelines,
							defaultPipelineConfiguration)
						.Invoke);
			return builder;
		}
	}
}