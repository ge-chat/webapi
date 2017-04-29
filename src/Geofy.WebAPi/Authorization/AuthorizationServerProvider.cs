using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Extensions;
using AspNet.Security.OpenIdConnect.Server;
using Geofy.Services;
using Geofy.Shared.Resources;
using Microsoft.AspNet.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;

namespace Geofy.WebAPi.Authorization
{
    public class AuthorizationServerProvider : OpenIdConnectServerProvider
    {
        public override Task ValidateClientAuthentication(ValidateClientAuthenticationContext context)
        {
            //not validate client secret
            context.Skipped();
            return Task.CompletedTask;
        }

        public override async Task GrantResourceOwnerCredentials(GrantResourceOwnerCredentialsContext context)
        {
            var authenticationService = context.HttpContext.RequestServices.GetService<AuthenticationService>();
            var user = await authenticationService.ValidateUser(context.UserName, context.Password);
            if (user == null)
            {
                context.Rejected(MessageConstants.Errors.UserInvalidCredentials);
                return;
            }

            var identity = new ClaimsIdentity(OpenIdConnectDefaults.AuthenticationScheme);
            identity.AddClaim(ClaimTypes.NameIdentifier, user.Id);
            identity.AddClaim(ClaimTypes.Name, user.UserName);
            identity.AddClaim(ClaimTypes.Email, user.Email);
            context.Validated(new ClaimsPrincipal(identity));
        }
    }
}