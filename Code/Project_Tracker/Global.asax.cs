using System.Web;
using System.Web.Mvc;

namespace Project_Tracker {
    public class MvcApplication : HttpApplication {
        protected void Application_Start() {
            AreaRegistration.RegisterAllAreas();
        }
    }
}