using System;
using System.Web.Mvc;
using CommonUtils.Code;
using MainLogic;
using MainLogic.WebFiles;

namespace Project_R.Controllers {
    public class HomeController : ApplicationControllerBase {
        public ActionResult Index() {
            return View();
        }

        const long SecondsToCaptchaValid = 10 * 10000000L;

        [HttpPost]
        public ActionResult SendRequest(string name, string email, string body, long? captcha) {
            if (!captcha.HasValue || (TicksNow - captcha) > SecondsToCaptchaValid) {
                return new EmptyResult();
            }
            new Mailer().SendSafe("admin@re-dan.ru", "", string.Format(""), string.Format(""));
            return new JsonResult {
                Data = true
            };
        }

        public ActionResult GetCaptcha() {
            return new JsonResult {
                Data = TicksNow.ToString()
            };
        }

        private static long TicksNow => DateTime.Now.Ticks;
    }
}