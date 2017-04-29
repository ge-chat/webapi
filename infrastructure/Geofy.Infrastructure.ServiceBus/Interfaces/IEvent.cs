namespace Geofy.Infrastructure.ServiceBus.Interfaces
{
    /// <summary>
    /// Domain Event
    /// </summary>
    public interface IEvent
    {
        IEventMetadata Metadata { get; set; }
    }
}