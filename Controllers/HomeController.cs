
using System.Web.Mvc;

namespace ProjectCSA.Controllers

{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Login";
            return View();
        }
    }
}