using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using MainLogic.WebFiles;
using Project_B.CodeServerSide.Enums;

namespace Project_B.Models {
    public class ExternalLoginConfirmationViewModel {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel {
        public string SelectedProvider { get; set; }
        public ICollection<SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }

        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel : StaticPageBaseModel<object> {
        public LoginViewModel(BaseModel baseModel) : base(LanguageType.Default, PageType.Undefined, baseModel) {
        }

        public LoginViewModel(LanguageType languageType, PageType pageType, BaseModel baseModel)
            : base(languageType, pageType, baseModel) {
        }

        public LoginViewModel(LanguageType languageType, PageType pageType, BaseModel baseModel,
            LoginViewModel loginModel) : base(languageType, pageType, baseModel) {
            Email = loginModel.Email;
            Password = loginModel.Password;
            RememberMe = loginModel.RememberMe;
        }

        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel : StaticPageBaseModel<object> {
        public RegisterViewModel(BaseModel baseModel) : base(LanguageType.Default, PageType.Undefined, baseModel) {
        }

        public RegisterViewModel(LanguageType languageType, PageType pageType, BaseModel baseModel)
            : base(languageType, pageType, baseModel) {
        }

        public RegisterViewModel(LanguageType languageType, PageType pageType, BaseModel baseModel,
            RegisterViewModel loginModel) : base(languageType, pageType, baseModel) {
            Email = loginModel.Email;
            Password = loginModel.Password;
            ConfirmPassword = loginModel.ConfirmPassword;
        }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ResetPasswordViewModel : StaticPageBaseModel<object> {
        public ResetPasswordViewModel(BaseModel baseModel) : base(LanguageType.Default, PageType.Undefined, baseModel) {
        }

        public ResetPasswordViewModel(LanguageType languageType, PageType pageType, BaseModel baseModel)
            : base(languageType, pageType, baseModel) {
        }

        public ResetPasswordViewModel(LanguageType languageType, PageType pageType, BaseModel baseModel,
            ResetPasswordViewModel loginModel) : base(languageType, pageType, baseModel) {
            Email = loginModel.Email;
            Password = loginModel.Password;
            ConfirmPassword = loginModel.ConfirmPassword;
            Code = loginModel.Code;
        }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [System.ComponentModel.DataAnnotations.Compare("Password",
            ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel : StaticPageBaseModel<object> {
        public ForgotPasswordViewModel(BaseModel baseModel) : base(LanguageType.Default, PageType.Undefined, baseModel) {
        }

        public ForgotPasswordViewModel(LanguageType languageType, PageType pageType, BaseModel baseModel)
            : base(languageType, pageType, baseModel) {
        }

        public ForgotPasswordViewModel(LanguageType languageType, PageType pageType, BaseModel baseModel,
            ResetPasswordViewModel loginModel) : base(languageType, pageType, baseModel) {
            Email = loginModel.Email;
        }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}