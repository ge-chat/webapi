using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;

namespace Geofy.WebAPi.SignalR
{
    public class TestHub : Hub
    {
        public void TestMessage(string str)
        {
            Clients.User(Context.User.Identity.Name).messageReceived(str);
        }
    }
}