using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Geofy.Shared.Resources;

namespace Geofy.WebAPi.ViewModels.User
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = MessageConstants.Errors.EmailRequired)]
        [EmailAddress(ErrorMessage = MessageConstants.Errors.EmailInvalid)]
        public string Email { get; set; }

        [Required(ErrorMessage = MessageConstants.Errors.PasswordRequired)]
        public string Password { get; set; } 
    }
}