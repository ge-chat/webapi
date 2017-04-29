using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Extensions;
using AspNet.Security.OpenIdConnect.Server;
using Geofy.Services;
using Geofy.Shared.Resources;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace Geofy.WebAPi.Authorization
{
    public class AuthorizationServerProvider : OpenIdConnectServerProvider
    {
        public override Task ValidateTokenRequest(ValidateTokenRequestContext context)
        {
            //not validate client secret
            context.Skip();
            return Task.CompletedTask;
        }

        public override async Task GrantResourceOwnerCredentials(GrantResourceOwnerCredentialsContext context)
        {
            var authenticationService = context.HttpContext.RequestServices.GetService<AuthenticationService>();
            var user = await authenticationService.ValidateUser(context.UserName, context.Password);
            if (user == null)
            {
                context.Reject(MessageConstants.Errors.UserInvalidCredentials);
                return;
            }

            var identity = new ClaimsIdentity(OpenIdConnectDefaults.AuthenticationScheme);
            identity.AddClaim(ClaimTypes.NameIdentifier, user.Id,
                OpenIdConnectConstants.Destinations.AccessToken,
                OpenIdConnectConstants.Destinations.IdentityToken);
            identity.AddClaim(ClaimTypes.Name, user.Email,
                OpenIdConnectConstants.Destinations.AccessToken,
                OpenIdConnectConstants.Destinations.IdentityToken);

            var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity),
                new AuthenticationProperties(),
                context.Options.AuthenticationScheme);
            ticket.SetResources(new [] { "http://192.168.55.2:5000/", "http://localhost:5000/" });

            context.Validate(ticket);
        }
    }
}