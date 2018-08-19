using System.Web.Mvc;

namespace ZondervanLibrary.Statistics.Web.Controllers
{
    public class AlumniMagazineController : Controller
    {
        [Route("alumni-magazine")]
        [Route("alumni-magazines", Order = 1)]
        public ActionResult Index()
        {
            return View();
        }
    }
}