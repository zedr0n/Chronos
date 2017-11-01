using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Net.Tracking.Commands
{
    public class StartTrackingHandler : ICommandHandler<StartTrackingCommand>
    {
        private readonly IDomainRepository _repository;

        public StartTrackingHandler(IDomainRepository repository)
        {
            _repository = repository;
        }

        public void Handle(StartTrackingCommand command)
        {
            var tracker = _repository.Find<Tracker>(command.TargetId) ?? new Tracker();
            tracker.StartTracking(command.AssetId);
            _repository.Save(tracker);
        }
    }
}