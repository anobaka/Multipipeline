namespace Anobaka.Multipipeline.DecisionTree
{
    public class PipelineDecisionNode
    {
	    public PipelineDecisionNode Parent { get; set; }
	    public IPipeline Current { get; set; }
    }
}
