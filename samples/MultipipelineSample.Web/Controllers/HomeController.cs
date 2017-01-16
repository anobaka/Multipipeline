using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LazyMortal.Multipipeline;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MultipipelineSample.Web.Controllers
{
	public class HomeController : Controller
	{
		private const string SessionKey = "key";

		public string Index()
		{
			var flag = HttpContext.Session.GetString(SessionKey);
			if (string.IsNullOrEmpty(flag))
			{
				flag = $"This is session in pipeline: {HttpContext.GetPipeline()?.Name}";
				HttpContext.Session.SetString(SessionKey, flag);
			}
			return flag;
		}
	}
}