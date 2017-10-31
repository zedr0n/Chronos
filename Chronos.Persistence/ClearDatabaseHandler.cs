using System.IO;
using Chronos.Infrastructure.Commands;

namespace Chronos.Persistence
{
    public class ClearDatabaseHandler : ICommandHandler<ClearDatabaseCommand>
    {
        private readonly IEventDb _eventDb;
        private readonly IReadDb _readDb;
        private readonly StreamTracker _streamTracker;

        public ClearDatabaseHandler(IReadDb readDb, IEventDb eventDb, StreamTracker streamTracker)
        {
            _readDb = readDb;
            _eventDb = eventDb;
            _streamTracker = streamTracker;
        }

        public void Handle(ClearDatabaseCommand command)
        {
            _readDb.Clear();
            _eventDb.Clear();
            _streamTracker.Clear();
        }
    }
}