using System;
using Geofy.Infrastructure.ServiceBus.Messages;

namespace Geofy.Domain.Commands.Chart
{
    public class PostMessage : Command
    {
        public string MessageId { get; set; }
        public string UserId { get; set; }
        public string ChartId { get; set; }
        public DateTime Created { get; set; }
        public string Message { get; set; }
    }
}