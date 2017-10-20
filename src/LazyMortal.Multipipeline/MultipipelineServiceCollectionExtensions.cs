using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LazyMortal.Multipipeline.DecisionTree;
using Microsoft.Extensions.DependencyInjection;

namespace LazyMortal.Multipipeline
{
	public static class MultipipelineServiceCollectionExtensions
	{
        /// <summary>
        /// <para>Use <see cref="MultipipelineOptions"/> as default options. To use custom options, try <see cref="AddMultipipeline{TOptions}"/>.</para>
        /// <para>All non-abstract and public types in entry assembly will be found and added to the pipeline collection.</para>
        /// </summary>
        /// <param name="services"></param>
        /// <param name="pipelines"></param>
        /// <param name="configureAction"></param>
        /// <returns></returns>
		public static IServiceCollection AddMultipipeline(this IServiceCollection services, List<IPipeline> pipelines,
			Action<MultipipelineOptions> configureAction = null) => services.AddMultipipeline<MultipipelineOptions>(pipelines, configureAction);

	    /// <summary>
	    /// All non-abstract and public types in entry assembly will be found, registered and added to the pipeline collection.
	    /// </summary>
	    /// <typeparam name="TOptions"></typeparam>
	    /// <param name="services"></param>
	    /// <param name="pipelines"></param>
	    /// <param name="configureAction"></param>
	    /// <returns></returns>
	    public static IServiceCollection AddMultipipeline<TOptions>(this IServiceCollection services, List<IPipeline> pipelines,
			Action<TOptions> configureAction = null) where TOptions : MultipipelineOptions, new()
		{
			if (configureAction != null)
			{
				services.Configure(configureAction);
			}
			services.AddSingleton<PipelineDecisionTree>();
			//var baseType = typeof(IPipeline);
			//var pipelines =
			//	Assembly.GetEntryAssembly()
			//		.DefinedTypes.Where(t => baseType.IsAssignableFrom(t.AsType()) && !t.IsAbstract)
			//		.Select(t => Activator.CreateInstance(t.AsType()) as IPipeline)
			//		.ToList();
			services.AddSingleton(new PipelineCollectionAccessor {Pipelines = pipelines});
			return services;
		}
	}
}