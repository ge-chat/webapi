using System;
using System.Threading.Tasks;

namespace Geofy.Infrastructure.ServiceBus.Interfaces
{
    /// <summary>
    /// Messages bus for commands
    /// </summary>
    public interface ICommandBus
    {
        Task SendAsync(params ICommand[] commands);
        Task SendAsync<TCommand>(Action<TCommand> builder) where TCommand : ICommand, new();
    }
}