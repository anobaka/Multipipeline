using System.Collections.Generic;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Reflection;

namespace LazyMortal.Multipipeline.DecisionTree
{
	/// <summary>
	/// Represent the relationships of all pipelines.
	/// </summary>
	public class PipelineDecisionTree
	{
		private readonly IDictionary<IPipeline, PipelineDecisionNode> _pipelineNodes;

		public PipelineDecisionTree(PipelineCollectionAccessor pipelineCollectionAccessor)
		{
			_pipelineNodes = pipelineCollectionAccessor.Pipelines?.ToDictionary(t => t, t => new PipelineDecisionNode {Current = t});
			_initTree();
		}

		public IList<IPipeline> GetPipelinePath(IPipeline pipeline)
		{
			return GetPipelinePath<IPipeline>(pipeline);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="pipeline"></param>
		/// <returns><paramref name="pipeline"/> will be added to the head of path, order by child -> parent</returns>
		public IList<TCast> GetPipelinePath<TCast>(TCast pipeline) where TCast : IPipeline
		{
			var list = new List<IPipeline> {pipeline};
			var parent = _pipelineNodes[pipeline].Parent;
			while (parent != null)
			{
				list.Add(parent.Current);
				parent = parent.Parent;
			}
			return list.Cast<TCast>().ToList();
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