using System;
using System.Threading.Tasks;
using Geofy.Infrastructure.ServiceBus.Interfaces;
using Geofy.Infrastructure.ServiceBus.Logging;

namespace Geofy.Infrastructure.ServiceBus.RabbitMq
{
    /// <summary>
    /// Messages bus for commands
    /// </summary>
    public class CommandBus : ICommandBus
    {
        private readonly RabbitMqClientsContainer<ICommand> _mqClientsContainer;
        private readonly ILoggingService _log;

        public CommandBus(RabbitMqClientsContainer<ICommand> mqClientsContainer, ILogFactory logFactory)
        {
            _mqClientsContainer = mqClientsContainer;
            _log = logFactory.GetLoggingService(ToString());
        }

        /// <summary>
        /// SendAsync single or several messages
        /// </summary>
        public async Task SendAsync(params ICommand[] commands)
        {
            PrepareCommands(commands);

            try
            {
                foreach (var command in commands)
                {
                    await _mqClientsContainer.GetMesasgeQueueClient(command).SendAsync(command);
                }
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                // we are not throwing exception here, because dispatching 
                // may be performed asynchronously and on another machine
                // (but right now we dispatching synchronously)
                // so we can just log error message
                _log.Error(ex);
            }
        }

        /// <summary>
        /// SendAsync single message using builder
        /// </summary>
        public Task SendAsync<TCommand>(Action<TCommand> builder) where TCommand : ICommand, new()
        {
            var command = new TCommand();
            builder(command);
            return SendAsync(command);
        }
     
        /// <summary>
        /// Prepare commands before they reach adressee
        /// </summary>
        private void PrepareCommands(params ICommand[] commands)
        {
            foreach (ICommand command in commands)
            {
                command.Metadata.CommandId = Guid.NewGuid().ToString();
                command.Metadata.CreatedDate = DateTime.UtcNow;
                command.Metadata.TypeName = command.GetType().FullName;
            }
        }
    }
}