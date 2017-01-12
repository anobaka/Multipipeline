using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anobaka.Multipipeline;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MultipipelineSample.Web.Controllers
{
	public class HomeController : Controller
	{
		private const string SessionKey = "key";

		public string Index()
		{
			return HttpContext.Session.GetString(SessionKey);
		}

		public void SetSession()
		{
			HttpContext.Session.SetString(SessionKey, $"{HttpContext.GetPipeline()?.Name} Session");
		}
	}
}