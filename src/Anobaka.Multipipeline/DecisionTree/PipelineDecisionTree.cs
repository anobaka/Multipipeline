using System.Collections.Generic;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Reflection;

namespace Anobaka.Multipipeline.DecisionTree
{
	public class PipelineDecisionTree<TOptions> where TOptions : MultipipelineOptions, new()
	{
		private readonly IDictionary<IPipeline, PipelineDecisionNode> _pipelineNodes;

		public PipelineDecisionTree(IOptions<TOptions> options)
		{
			_pipelineNodes = options.Value.Pipelines?.ToDictionary(t => t, t => new PipelineDecisionNode {Current = t});
			_initTree();
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="pipeline"></param>
		/// <returns><paramref name="pipeline"/> will be set in path</returns>
		public IList<IPipeline> GetPipelinePath(IPipeline pipeline)
		{
			var list = new List<IPipeline> {pipeline};
			var parent = _pipelineNodes[pipeline].Parent;
			while (parent != null)
			{
				list.Add(parent.Current);
				parent = parent.Parent;
			}
			return list;
		}

		private void _initTree()
		{
			var types = _pipelineNodes.ToDictionary(t => t.Key, t => t.Key.GetType().GetTypeInfo());
			foreach (var node in _pipelineNodes)
			{
				if (node.Value.Parent == null)
				{
					foreach (var otherNode in _pipelineNodes)
					{
						if (otherNode.Key != node.Key && types[node.Key].BaseType == otherNode.Key.GetType())
						{
							node.Value.Parent = otherNode.Value;
							break;
						}
					}
				}
			}
		}
	}
}