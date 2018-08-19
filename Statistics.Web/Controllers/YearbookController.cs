using System.Web.Mvc;

namespace ZondervanLibrary.Statistics.Web.Controllers
{
    public class YearbookController : Controller
    {
        [Route("yearbook")]
        [Route("yearbooks", Order = 1)]
        public ActionResult Index()
        {
            return View();
        }
	}
}