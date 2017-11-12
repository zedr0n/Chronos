using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Net.Tracking.Commands
{
    public class StopTrackingHandler : ICommandHandler<StopTrackingCommand>
    {
        private readonly IDomainRepository _repository;

        public StopTrackingHandler(IDomainRepository repository)
        {
            _repository = repository;
        }

        public void Handle(StopTrackingCommand command)
        {
            var tracker = _repository.Find<Tracker>(command.TargetId) ?? new Tracker();
            tracker.StopTracking(command.AssetId);
            _repository.Save(tracker); 
        }
    }
}