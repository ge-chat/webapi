using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Geofy.WebAPi.ViewModels.User
{
    public class LoginViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, PasswordPropertyText]
        public string Password { get; set; } 
    }
}