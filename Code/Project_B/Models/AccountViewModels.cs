using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using MainLogic.WebFiles;
using Project_B.CodeClientSide;
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

    public class LoginViewModel : StaticPageBaseModel {
        public LoginViewModel(BaseModel baseModel) : base(baseModel) {
        }

        public LoginViewModel(ProjectControllerBase projectControllerBase)
            : base(projectControllerBase) {
        }

        public LoginViewModel(ProjectControllerBase projectControllerBase, LoginViewModel loginModel) : base(projectControllerBase) {
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

    public class RegisterViewModel : StaticPageBaseModel {
        public RegisterViewModel(BaseModel baseModel) : base(baseModel) {
        }

        public RegisterViewModel(ProjectControllerBase projectControllerBase) : base(projectControllerBase) {
        }

        public RegisterViewModel(ProjectControllerBase projectControllerBase, RegisterViewModel loginModel) : base(projectControllerBase) {
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

    public class ResetPasswordViewModel : StaticPageBaseModel {
        public ResetPasswordViewModel(BaseModel baseModel) : base(baseModel) {
        }

        public ResetPasswordViewModel(ProjectControllerBase projectControllerBase)
            : base(projectControllerBase) {
        }

        public ResetPasswordViewModel(ProjectControllerBase projectControllerBase, ResetPasswordViewModel loginModel) : base(projectControllerBase) {
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

    public class ForgotPasswordViewModel : StaticPageBaseModel {
        public ForgotPasswordViewModel(BaseModel baseModel) : base(baseModel) {
        }

        public ForgotPasswordViewModel(ProjectControllerBase projectControllerBase)
            : base(projectControllerBase) {
        }

        public ForgotPasswordViewModel(ProjectControllerBase projectControllerBase,
            ResetPasswordViewModel loginModel) : base(projectControllerBase) {
            Email = loginModel.Email;
        }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}