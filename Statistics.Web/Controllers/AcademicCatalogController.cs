using System.Web.Mvc;

namespace ZondervanLibrary.Statistics.Web.Controllers
{
    public class AcademicCatalogController : Controller
    {
        [Route("academic-catalog")]
        [Route("academic-catalogs", Order = 1)]
        public ActionResult Index()
        {
            return View();
        }
    }
}