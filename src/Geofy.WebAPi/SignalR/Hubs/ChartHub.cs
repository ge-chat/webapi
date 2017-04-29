using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Geofy.WebAPi.SignalR.Hubs
{
    public class ChartHub : Hub
    {
        private const string CHAT_KEY_PARAM = "chatId";

        public override Task OnConnected()
        {
            var chartId = Context.QueryString[CHAT_KEY_PARAM].FirstOrDefault();
            return !string.IsNullOrEmpty(chartId)
                ? Groups.Add(Context.ConnectionId, chartId)
                : Task.CompletedTask;
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var chartId = Context.QueryString[CHAT_KEY_PARAM].FirstOrDefault();
            return !string.IsNullOrEmpty(chartId) 
                ? Groups.Remove(Context.ConnectionId, chartId) :
                Task.CompletedTask;
        }

        public async Task connectToChat(string connectionId, IEnumerable<string> chartIds)
        {
            foreach (var chartId in chartIds)
            {
                await Groups.Add(connectionId, chartId);
            }
        }

        public async Task disconnectFromChat(string connectionId, IEnumerable<string> chartIds)
        {
            foreach (var chartId in chartIds)
            {
                await Groups.Remove(connectionId, chartId);
            }
        }
    }
}