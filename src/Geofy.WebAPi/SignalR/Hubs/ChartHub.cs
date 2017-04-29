using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Extensions;
using Microsoft.AspNetCore.SignalR;

namespace Geofy.WebAPi.SignalR.Hubs
{
    public class ChartHub : Hub
    {
        public override Task OnConnected()
        {
            var identity = (ClaimsIdentity) Context.User.Identity;
            var groupId = identity.GetClaim(ClaimTypes.NameIdentifier);
            return !string.IsNullOrEmpty(groupId)
                ? Groups.Add(Context.ConnectionId, groupId)
                : Task.CompletedTask;
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var identity = (ClaimsIdentity)Context.User.Identity;
            var groupId = identity.GetClaim(ClaimTypes.NameIdentifier);
            return !string.IsNullOrEmpty(groupId) 
                ? Groups.Remove(Context.ConnectionId, groupId) :
                Task.CompletedTask;
        }
    }
}