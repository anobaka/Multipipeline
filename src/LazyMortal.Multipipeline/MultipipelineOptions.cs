using System.Collections.Generic;

namespace LazyMortal.Multipipeline
{
    public class MultipipelineOptions
    {
	    public string PipelineHttpContextItemKey { get; set; } = "Pipeline";
	    public List<IPipeline> Pipelines { get; set; }
    }
}
