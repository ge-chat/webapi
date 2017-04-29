namespace Geofy.Infrastructure.ServiceBus.Interfaces
{
    /// <summary>
    /// Domain Event
    /// </summary>
    public interface IEvent
    {
        string Id { get; set; }

        IEventMetadata Metadata { get; set; }
    }
}