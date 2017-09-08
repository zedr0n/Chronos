using Chronos.Infrastructure.Commands;

namespace Chronos.Persistence
{
    public class ClearDatabaseHandler : ICommandHandler<ClearDatabaseCommand>
    {
        private readonly IEventDb _eventDb;
        private readonly IReadDb _readDb;

        public ClearDatabaseHandler(IReadDb readDb, IEventDb eventDb)
        {
            _readDb = readDb;
            _eventDb = eventDb;
        }

        public void Handle(ClearDatabaseCommand command)
        {
            _readDb.Clear();
            _eventDb.Clear();
        }
    }
}