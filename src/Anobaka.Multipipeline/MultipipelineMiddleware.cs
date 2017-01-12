using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Anobaka.Multipipeline.DecisionTree;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;

namespace Anobaka.Multipipeline
{
	public class MultipipelineMiddleware<TOptions> where TOptions : MultipipelineOptions, new()
	{
		private readonly RequestDelegate _next;
		private readonly ILogger _logger;
		private readonly IOptions<TOptions> _options;
		private readonly IApplicationBuilder _app;
		private static readonly EventId InformationEventId = new EventId(10000, nameof(MultipipelineMiddleware<TOptions>));
		private readonly PipelineDecisionTree<TOptions> _decisionTree;

		private readonly ConcurrentDictionary<IPipeline, Lazy<RequestDelegate>> _pipelines =
			new ConcurrentDictionary<IPipeline, Lazy<RequestDelegate>>();

		public MultipipelineMiddleware(RequestDelegate next, IApplicationBuilder app, ILoggerFactory loggerFactory,
			IOptions<TOptions> options, PipelineDecisionTree<TOptions> decisionTree)
		{
			_next = next;
			_logger = loggerFactory.CreateLogger(nameof(MultipipelineMiddleware<TOptions>));
			_options = options;
			_decisionTree = decisionTree;
			_app = app;
		}

		public async Task Invoke(HttpContext httpContext)
		{
			var deepestDepth = 0;
			IPipeline pipeline = null;
			foreach (var p in _options.Value.Pipelines)
			{
				if (await p.ResolveAsync(httpContext))
				{
					var depth = _decisionTree.GetPipelinePath(p).Count;
					if (depth > deepestDepth)
					{
						pipeline = p;
						deepestDepth = depth;
					}
				}
			}
			if (pipeline != null)
			{
				httpContext.Items[_options.Value.PipelineHttpContextItemKey] = pipeline;
				_logger.LogInformation(InformationEventId, $"pipeline resolved: {pipeline.Name}");
				var requestDelegate = _pipelines.GetOrAdd(pipeline,
					new Lazy<RequestDelegate>(() =>
					{
						var builder = _app.New();
						Task.WaitAll(pipeline.ConfigurePipeline(builder));
						builder.Run(_next);
						return builder.Build();
					}));
				await requestDelegate.Value(httpContext);
			}
			else
			{
				await _next(httpContext);
			}
		}
	}

	public static class CooperationMiddlewareExtensions
	{
		/// <summary>
		/// use <see cref="MultipipelineOptions"/> as default pipeline options. To use custom options, try <see cref="AddMultipipeline{TOptions}"/>
		/// </summary>
		public static IApplicationBuilder AddMultipipeline(this IApplicationBuilder builder)
		{
			return builder.AddMultipipeline<MultipipelineOptions>();
		}

		public static IApplicationBuilder AddMultipipeline<TOptions>(this IApplicationBuilder builder)
			where TOptions : MultipipelineOptions, new()
		{
			var options = (IOptions<TOptions>) builder.ApplicationServices.GetService(typeof(IOptions<TOptions>));
			if (options == null)
			{
				throw new ArgumentNullException(nameof(TOptions),
					"use services.Configure<TOptions> first");
			}
			var decisionTree = builder.ApplicationServices.GetRequiredService<PipelineDecisionTree<TOptions>>();
			builder.Use(
				t =>
					new MultipipelineMiddleware<TOptions>(t, builder,
						(ILoggerFactory) builder.ApplicationServices.GetService(typeof(ILoggerFactory)), options, decisionTree).Invoke);
			return builder;
		}
	}
}