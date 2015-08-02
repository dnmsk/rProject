using System.Web.Mvc;

namespace Project_R.Controllers {
    public class HomeController : Controller {
        public ActionResult Index() {
            return View();
        }

        [HttpPost]
        public ActionResult SendRequest(string name, string email, string body) {
            
            return new EmptyResult();
        }
    }
}