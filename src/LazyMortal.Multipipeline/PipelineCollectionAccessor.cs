using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LazyMortal.Multipipeline
{
	/// <summary>
	/// Get all registered pipelines' instances.
	/// </summary>
    public class PipelineCollectionAccessor
    {
	    public IList<IPipeline> Pipelines { get; set; }
    }
}
