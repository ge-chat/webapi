using System;
using Geofy.Infrastructure.ServiceBus.Interfaces;

namespace Geofy.Infrastructure.ServiceBus.Messages
{
    /// <summary>
    /// Domain Command
    /// </summary>
    public abstract class Command : ICommand
    {
        /// <summary>
        /// Command metadata
        /// </summary>
        public ICommandMetadata Metadata { get; set; } = new CommandMetadata();
    }

    public class CommandMetadata : ICommandMetadata
    {
        /// <summary>
        /// Unique Id of Command
        /// </summary>
        public string CommandId { get; set; }

        /// <summary>
        /// User Id of user who initiate this command
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Assembly qualified CLR Type name of Command Type
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// Time when command was created
        /// </summary>
        public DateTime CreatedDate { get; set; }
    }
}
