using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace LazyMortal.Multipipeline
{
    public class MultipipelineOptions
    {
		/// <summary>
		/// Default value is "Pipeline", used to store current pipeline in <see cref="HttpContext.Items"/>
		/// </summary>
	    public string PipelineHttpContextItemKey { get; set; } = "Pipeline";
    }
}
