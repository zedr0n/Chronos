namespace Chronos.Persistence
{
    public class EventDb : BaseDb<EventContext>, IEventDb
    {
        public EventDb(string dbName, bool isPersistent, bool inMemory)
            : base(dbName,isPersistent, inMemory) { }

    }
}