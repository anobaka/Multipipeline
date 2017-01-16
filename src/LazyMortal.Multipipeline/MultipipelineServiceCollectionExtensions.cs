using System;
using System.Linq;
using System.Reflection;
using LazyMortal.Multipipeline.DecisionTree;
using Microsoft.Extensions.DependencyInjection;

namespace LazyMortal.Multipipeline
{
	public static class MultipipelineServiceCollectionExtensions
	{
		/// <summary>
		/// Use <see cref="MultipipelineOptions"/> as default options. To use custom options, try <see cref="AddMultipipeline{TOptions}"/>
		/// </summary>
		public static IServiceCollection AddMultipipeline(this IServiceCollection services,
			Action<MultipipelineOptions> configureAction = null)
		{
			return services.AddMultipipeline<MultipipelineOptions>(configureAction);
		}

		public static IServiceCollection AddMultipipeline<TOptions>(this IServiceCollection services,
			Action<TOptions> configureAction = null) where TOptions : MultipipelineOptions, new()
		{
			if (configureAction != null)
			{
				services.Configure(configureAction);
			}
			services.AddSingleton<PipelineDecisionTree>();
			var baseType = typeof(IPipeline);
			var pipelines =
				Assembly.GetEntryAssembly()
					.DefinedTypes.Where(t => baseType.IsAssignableFrom(t.AsType()) && !t.IsAbstract)
					.Select(t => Activator.CreateInstance(t.AsType()) as IPipeline)
					.ToList();
			services.AddSingleton(new PipelineCollectionAccessor {Pipelines = pipelines});
			return services;
		}
	}
}