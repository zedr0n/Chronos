using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Interfaces;

namespace Chronos.Infrastructure.Commands
{
    public class TrackJsonHandler<T> : ICommandHandler<TrackJsonCommand<T>>
    {
        private readonly IEventStore _eventStore;

        public TrackJsonHandler(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public void Handle(TrackJsonCommand<T> command)
        {
            _eventStore.Alert(
                new JsonTrackingRequested<T>
                {
                    RequestId = command.RequestId,
                    UpdateInterval = command.UpdateInterval,
                    Url = command.Url
                });
            //_connection.Writer.AppendToNull(events);
        }
    }
}