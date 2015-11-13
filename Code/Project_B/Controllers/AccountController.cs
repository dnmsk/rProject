﻿using System.Web;
using System.Web.Mvc;
using MainLogic.WebFiles;
using Microsoft.Owin.Security;
using Project_B.CodeClientSide;
using Project_B.Models;

namespace Project_B.Controllers {
    [Authorize]
    public class AccountController : ProjectControllerBase {
        //
        // GET: /Account/Login
        [AllowAnonymous]
        [ActionLog(ProjectBActions.PageAccountLoginIndex)]
        public ActionResult Login(string returnUrl) {
            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginViewModel(this));
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [ActionLog(ProjectBActions.PageAccountLoginPost)]
        public ActionResult Login(LoginViewModel model, string returnUrl) {
            var loginModel = new LoginViewModel(this, model);
            if (!ModelState.IsValid) {
                return View(loginModel);
            }

            var loginResult = BusinessLogic.AccountProvider.LoginWithEmail(model.Email, model.Password);
            if (loginResult != null) {
                CurrentUser = new SessionModule((int)loginResult.Item1, loginResult.Item2);
                return RedirectToLocal(returnUrl);
            }

            ModelState.AddModelError("", "Invalid login attempt.");
            return View(loginModel);
            /*
            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = SignInManager.PasswordSignIn(loginModel.Email, loginModel.Password, loginModel.RememberMe, false);
            switch (result) {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new {ReturnUrl = returnUrl, loginModel.RememberMe});
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(loginModel);
            }
            */
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public ActionResult VerifyCode(string provider, string returnUrl, bool rememberMe) {
            // Require that the user has already logged in via username/password or external login
            /*
            if (!SignInManager.HasBeenVerified()) {
                return View("Error");
            }
            */
            return View(new VerifyCodeViewModel {Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe});
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult VerifyCode(VerifyCodeViewModel model) {
            if (!ModelState.IsValid) {
                return View(model);
            }
            /*
            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = SignInManager.TwoFactorSignIn(model.Provider, model.Code, model.RememberMe,
                        model.RememberBrowser);
            switch (result) {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
            */
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
        public ActionResult Register(RegisterViewModel model) {
            if (ModelState.IsValid) {
                var result = BusinessLogic.AccountProvider.RegisterWithEmail(CurrentUser.GuestID, model.Email, model.Password);// UserManager.Create(user, model.Password);
                if (result) {
                    var accountID = BusinessLogic.AccountProvider.LoginWithEmail(model.Email, model.Password);
                    CurrentUser = new SessionModule(CurrentUser.GuestID, accountID.Item2);
                    //SignInManager.SignIn(user, false, false);

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return RedirectToAction("Index", "Home");
                }
                AddErrors(new[] {
                    "Error register with email and password"
                });
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public ActionResult ConfirmEmail(int userId, string code) {
            if (userId == null || code == null) {
                //return View("Error");
            }
            //var result = UserManager.ConfirmEmail(userId, code);
            return null;//View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword() {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(ForgotPasswordViewModel model) {
            if (ModelState.IsValid) {
                /*
                var user = UserManager.FindByName(model.Email);
                if (user == null || !(UserManager.IsEmailConfirmed(user.Id))) {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }
                */
                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation() {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code) {
            return /*code == null ? View("Error") :*/ View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordViewModel model) {
            return null;
            /*
            if (!ModelState.IsValid) {
                return View(model);
            }
            var user = UserManager.FindByName(model.Email);
            if (user == null) {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = UserManager.ResetPassword(user.Id, model.Code, model.Password);
            if (result.Succeeded) {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
            */
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation() {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl) {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new {ReturnUrl = returnUrl}));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public ActionResult SendCode(string returnUrl, bool rememberMe) {
            return null;
            /*
            var userId = SignInManager.GetVerifiedUserId();
            if (userId == default(int)) {
                return View("Error");
            }
            var userFactors = UserManager.GetValidTwoFactorProviders(userId);
            var factorOptions =
                userFactors.Select(purpose => new SelectListItem {Text = purpose, Value = purpose}).ToList();
            return View(new SendCodeViewModel {Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe});
            */
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult SendCode(SendCodeViewModel model) {
            if (!ModelState.IsValid) {
                return View();
            }
            /*
            // Generate the token and send it
            if (!SignInManager.SendTwoFactorCode(model.SelectedProvider)) {
                return View("Error");
            }
            */
            return RedirectToAction("VerifyCode", new {Provider = model.SelectedProvider, model.ReturnUrl, model.RememberMe});
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public ActionResult ExternalLoginCallback(string returnUrl) {
            var loginInfo = AuthenticationManager.GetExternalLoginInfo();
            if (loginInfo == null) {
                return RedirectToAction("Login");
            }

            return RedirectToLocal(returnUrl);
            /*
            // Sign in the user with this external login provider if the user already has a login
            var result = SignInManager.ExternalSignIn(loginInfo, false);
            switch (result) {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new {ReturnUrl = returnUrl, RememberMe = false});
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation",
                        new ExternalLoginConfirmationViewModel {Email = loginInfo.Email});
            }
            */
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model,
            string returnUrl) {
            if (CurrentUser.IsAuthenticated()) {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid) {
                // Get the information about the user from the external login provider
                var info = AuthenticationManager.GetExternalLoginInfo();
                if (info == null) {
                    return View("ExternalLoginFailure");
                }
                /*
                var user = new ApplicationUser {
                    UserName = model.Email,
                    GuestId = CurrentUser.GuestID
                };
                var result = UserManager.Create(user);
                if (result.Succeeded) {
                    result = UserManager.AddLogin(user.Id, info.Login);
                    if (result.Succeeded) {
                        SignInManager.SignIn(user, false, false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
                */
            }

            ViewBag.ReturnUrl = returnUrl;
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

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure() {
            return View();
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                /*
                if (_userManager != null) {
                    _userManager.Dispose();
                    _userManager = null;
                }
                */
                /*
                if (_signInManager != null) {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
                */
            }

            base.Dispose(disposing);
        }

        #region Helpers

        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager {
            get { return HttpContext.GetOwinContext().Authentication; }
        }

        private void AddErrors(params string[] strings) {
            foreach (var error in strings) {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl) {
            if (Url.IsLocalUrl(returnUrl)) {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null) {
            }

            public ChallengeResult(string provider, string redirectUri, string userId) {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context) {
                var properties = new AuthenticationProperties {RedirectUri = RedirectUri};
                if (UserId != null) {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }

        #endregion
    }
}