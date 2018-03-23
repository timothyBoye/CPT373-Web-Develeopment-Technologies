using System.Web.Mvc;

namespace GoLA2.Controllers
{
    public class HomeController : Controller
    {

        /// <summary>
        /// Provides the Home page view
        /// </summary>
        /// <returns>Index View</returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Provides the About page view
        /// </summary>
        /// <returns>About page view</returns>
        public ActionResult About()
        {
            return View();
        }
    }
}