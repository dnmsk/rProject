using System.ComponentModel.DataAnnotations;
using MainLogic.WebFiles;

namespace Spywords_Project.Models {
    public class LoginViewModel : BaseModel {
        public LoginViewModel(BaseModel baseModel) : base(baseModel.SessionModule, baseModel.MainLogicProvider) {
        }

        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel : BaseModel {
        public RegisterViewModel(BaseModel baseModel) : base(baseModel.SessionModule, baseModel.MainLogicProvider) {
        }
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 3)]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Еще раз")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}