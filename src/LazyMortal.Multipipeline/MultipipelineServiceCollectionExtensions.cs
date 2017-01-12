using System;
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
			Action<MultipipelineOptions> configureAction)
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
			services.AddSingleton<PipelineDecisionTree<TOptions>>();
			return services;
		}
	}
}