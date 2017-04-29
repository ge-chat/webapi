using System.ComponentModel.DataAnnotations;
using Geofy.Shared.Resources;

namespace Geofy.WebAPi.ViewModels.User
{
    public class UserRegisterPostModel
    {
        [Required(ErrorMessage = MessageConstants.Errors.EmailRequired)]
        [EmailAddress(ErrorMessage = MessageConstants.Errors.EmailInvalid)]
        public string Email { get; set; }

        [Required(ErrorMessage = MessageConstants.Errors.PasswordRequired)]
        public string Password { get; set; }

        [Required(ErrorMessage = MessageConstants.Errors.ConfirmPasswordRequired)]
        [Compare(nameof(Password), ErrorMessage = MessageConstants.Errors.ConfirmPasswordRequired)]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = MessageConstants.Errors.UserNameRequired)]
        public string UserName { get; set; }
    }
}