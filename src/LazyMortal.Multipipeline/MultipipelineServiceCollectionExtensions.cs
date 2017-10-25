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
        /// </summary>
        /// <param name="services"></param>
        /// <param name="pipelines"></param>
        /// <param name="configureAction"></param>
        /// <returns></returns>
        public static IServiceCollection AddMultipipeline(this IServiceCollection services,
            IEnumerable<IPipeline> pipelines,
            Action<MultipipelineOptions> configureAction = null) =>
            services.AddMultipipeline<MultipipelineOptions>(t => pipelines, configureAction);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TOptions"></typeparam>
        /// <param name="services"></param>
        /// <param name="pipelines"></param>
        /// <param name="configureAction"></param>
        /// <returns></returns>
        public static IServiceCollection AddMultipipeline<TOptions>(this IServiceCollection services,
            IEnumerable<IPipeline> pipelines,
            Action<TOptions> configureAction = null) where TOptions : MultipipelineOptions, new()
        {
            return services.AddMultipipeline(t => pipelines, configureAction);
        }

        public static IServiceCollection AddMultipipeline(this IServiceCollection services,
            Func<IServiceProvider, IEnumerable<IPipeline>> implementationFactory,
            Action<MultipipelineOptions> configureAction = null) =>
            services.AddMultipipeline<MultipipelineOptions>(implementationFactory, configureAction);

        public static IServiceCollection AddMultipipeline<TOptions>(this IServiceCollection services,
            Func<IServiceProvider, IEnumerable<IPipeline>> implementationFactory,
            Action<TOptions> configureAction = null) where TOptions : MultipipelineOptions, new()
        {
            if (configureAction != null)
            {
                services.Configure(configureAction);
            }
            services.AddSingleton<PipelineDecisionTree>();

            services.AddSingleton(p =>
            {
                var pipelines = implementationFactory(p);
                var pipelinesList = pipelines.ToList();
                var invalidPipelineIds = pipelinesList.GroupBy(t => t.Id).Where(t => t.Count() > 1).ToList();
                if (invalidPipelineIds.Count > 0)
                {
                    throw new ArgumentException(
                        $"Pipelines with same id are found: {string.Join(", ", invalidPipelineIds.Select(t => $"[{t.Count()}]{t.Key}"))}",
                        nameof(pipelines));
                }
                return new PipelineCollectionAccessor {Pipelines = pipelinesList};
            });
            return services;
        }
    }
}