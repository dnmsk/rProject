using System.Threading.Tasks;
using System.Web.Mvc;
using MainLogic.WebFiles;
using Spywords_Project.Models;

namespace Spywords_Project.Controllers {
    [Authorize]
    public class AccountController : ApplicationControllerBase {
        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl) {
            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginViewModel(GetBaseModel()));
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl) {
            //ModelState.AddModelError("", error);
            if (!ModelState.IsValid) {
                return View(model);
            }
            
            var accountID = BusinessLogic.AccountProvider.LoginWithEmail(model.Email, model.Password);
            if (accountID != default(int)) {
                CurrentUser = new SessionModule(CurrentUser.GuestID, accountID);
                return RedirectToLocal(returnUrl);
            }
            ModelState.AddModelError("", "Неверный логин/пароль.");
            return View(model);
        }
        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register() {
            return View(new RegisterViewModel(GetBaseModel()));
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model) {
            if (ModelState.IsValid) {
                var result = BusinessLogic.AccountProvider.RegisterWithEmail(CurrentUser.GuestID, model.Email,
                    model.Password);
                if (result) {
                    var accountID = BusinessLogic.AccountProvider.LoginWithEmail(model.Email, model.Password);
                    CurrentUser = new SessionModule(CurrentUser.GuestID, accountID);
                    return RedirectToAction("Index", "Home");
                }
            }
            
            return View(model);
        }
        
        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff() {
            CurrentUser = null;
            return RedirectToAction("Index", "Home");
        }

        private ActionResult RedirectToLocal(string returnUrl) {
            if (Url.IsLocalUrl(returnUrl)) {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
    }
}