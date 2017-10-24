using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LazyMortal.Multipipeline.DecisionTree;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;

namespace LazyMortal.Multipipeline
{
    public class MultipipelineMiddleware<TOptions> where TOptions : MultipipelineOptions, new()
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly IOptions<TOptions> _options;
        private readonly IApplicationBuilder _app;

        private static readonly EventId InformationEventId =
            new EventId(10000, nameof(MultipipelineMiddleware<TOptions>));

        private readonly PipelineDecisionTree _decisionTree;
        private readonly PipelineCollectionAccessor _pipelineCollectionAccessor;
        private readonly Action<IApplicationBuilder> _defaultPipelineConfiguration;

        private readonly ConcurrentDictionary<IPipeline, Lazy<RequestDelegate>> _pipelines =
            new ConcurrentDictionary<IPipeline, Lazy<RequestDelegate>>();

        private RequestDelegate _defaultRequestDelegate;

        public MultipipelineMiddleware(RequestDelegate next, IApplicationBuilder app, ILoggerFactory loggerFactory,
            IOptions<TOptions> options, PipelineDecisionTree decisionTree,
            PipelineCollectionAccessor pipelineCollectionAccessor,
            Action<IApplicationBuilder> defaultPipelineConfiguration)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger(nameof(MultipipelineMiddleware<TOptions>));
            _options = options;
            _decisionTree = decisionTree;
            _app = app;
            _pipelineCollectionAccessor = pipelineCollectionAccessor;
            _defaultPipelineConfiguration = defaultPipelineConfiguration;
        }

        public virtual async Task Invoke(HttpContext httpContext)
        {
            var deepestDepth = 0;
            IPipeline pipeline = null;
            if (_pipelineCollectionAccessor.Pipelines?.Any() == true)
            {
                foreach (var p in _pipelineCollectionAccessor.Pipelines)
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
            }
            if (pipeline != null)
            {
                httpContext.Items[_options.Value.PipelineHttpContextItemKey] = pipeline;
                _logger.LogInformation(InformationEventId, $"Resolved pipeline: {pipeline.Name}");
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
                if (_defaultRequestDelegate == null && _defaultPipelineConfiguration != null)
                {
                    var builder = _app.New();
                    _defaultPipelineConfiguration(builder);
                    builder.Run(_next);
                    _defaultRequestDelegate = builder.Build();
                }
                await (_defaultRequestDelegate ?? _next)(httpContext);
            }
        }
    }
}