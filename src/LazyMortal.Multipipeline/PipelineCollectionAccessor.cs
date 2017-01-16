using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LazyMortal.Multipipeline
{
    public class PipelineCollectionAccessor
    {
	    public IList<IPipeline> Pipelines { get; set; }
    }
}
