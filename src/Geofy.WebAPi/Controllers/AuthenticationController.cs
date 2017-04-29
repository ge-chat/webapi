using System.Threading.Tasks;
using Geofy.Domain.Commands.User;
using Geofy.Services;
using Geofy.Shared.Resources;
using Geofy.WebAPi.ViewModels.User;
using Geofy.WebAPI.Services;
using Microsoft.AspNet.Mvc;

namespace Geofy.WebAPi.Controllers
{
    [Route("api/auth")]
    public class AuthenticationController : BaseController
    {
        private readonly AuthenticationService _authenticationService;

        public AuthenticationController(AuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]UserRegisterViewModel model)
        {
            var userExists = await _authenticationService.UserExists(model.Email);
            if (userExists)
            {
                ModelState.AddModelError("", MessageConstants.Errors.UserAlreadyRegistred);
            }
            if(!ModelState.IsValid) return new GeofyBadRequest(ModelState);

            var salt = _authenticationService.GenerateSalt();
            await CommandBus.SendAsync(new RegisterUser
            {
                UserId = IdGenerator.Generate(),
                Email = model.Email,
                PasswordHash = _authenticationService.GetHash(model.Password, salt),
                PasswordSalt = salt,
                UserName = model.UserName
            });

            return new HttpOkResult();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]LoginViewModel model)
        {
            var user = await _authenticationService.ValidateUser(model.Email, model.Password);
            if (user == null)
            {
                ModelState.AddModelError("", MessageConstants.Errors.UserInvalidCredentials);
            }
            if (!ModelState.IsValid) return new GeofyBadRequest(ModelState);
            return new HttpOkObjectResult(new {
                token = _authenticationService.GetToken(user)}
            );
        }
    }
}