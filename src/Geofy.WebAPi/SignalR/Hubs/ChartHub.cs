using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Extensions;
using Microsoft.AspNet.SignalR;

namespace Geofy.WebAPi.SignalR.Hubs
{
    public class ChartHub : Hub
    {
        public override Task OnConnected()
        {
            var identity = (ClaimsIdentity) Context.User.Identity;
            return Groups.Add(Context.ConnectionId, identity.GetClaim(ClaimTypes.NameIdentifier));
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var identity = (ClaimsIdentity)Context.User.Identity;
            return Groups.Remove(Context.ConnectionId, identity.GetClaim(ClaimTypes.NameIdentifier));
        }
    }
}