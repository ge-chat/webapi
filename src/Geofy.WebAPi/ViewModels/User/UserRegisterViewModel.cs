using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Geofy.WebAPi.ViewModels.User
{
    public class UserRegisterViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, PasswordPropertyText]
        public string Password { get; set; }

        [Required]
        public string UserName { get; set; }
    }
}