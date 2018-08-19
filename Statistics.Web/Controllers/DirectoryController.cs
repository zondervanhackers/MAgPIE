using System.Web.Mvc;

//using AttributeRouting.Web.Mvc;

namespace ZondervanLibrary.Statistics.Web.Controllers
{
    public class DirectoryController : Controller
    {
        [Route]
        [Route("a")]
        [Route("index")]
        public ActionResult Index()
        {
            return View();
        }
	}
}