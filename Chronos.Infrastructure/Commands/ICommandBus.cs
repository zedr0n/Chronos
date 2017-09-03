using System.Threading.Tasks;
using Chronos.Infrastructure.Interfaces;

namespace Chronos.Infrastructure.Commands
{
    public interface ICommandBus
    {
        void Send<T>(T e) where T : class,ICommand;
        Task SendAsync(ICommand command);
        void Send(ICommand command);
    }
}