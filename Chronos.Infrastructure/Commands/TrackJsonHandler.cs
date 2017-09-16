using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Interfaces;

namespace Chronos.Infrastructure.Commands
{
    public class TrackJsonHandler<T> : ICommandHandler<TrackJsonCommand<T>>
    {
        private readonly IEventStoreConnection _connection;

        public TrackJsonHandler(IEventStoreConnection connection)
        {
            _connection = connection;
        }

        public void Handle(TrackJsonCommand<T> command)
        {
            var events = new IEvent[]
            {
                new JsonTrackingRequested<T>
                {
                    RequestId = command.RequestId,
                    UpdateInterval = command.UpdateInterval,
                    Url = command.Url
                }
            };
            
            _connection.Writer.AppendToNull(events);
        }
    }
}