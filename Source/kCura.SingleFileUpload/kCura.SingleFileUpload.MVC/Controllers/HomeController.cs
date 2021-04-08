using System.Net;
using System.Web.Mvc;
using Relativity.Telemetry.APM;

namespace kCura.SingleFileUpload.MVC.Controllers
{
	public class HomeController : Controller
	{
		[HttpGet]
		public ActionResult HealthCheck()
		{
			Client.APMClient.HealthCheckOperation("SimpleFileUpload.HealthCheck", 
					() => new HealthCheckOperationResult(isHealthy: true, message: "Success!"))
				.Write();

			return new HttpStatusCodeResult(HttpStatusCode.OK);
		}
	}
}