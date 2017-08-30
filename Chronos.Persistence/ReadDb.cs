namespace Chronos.Persistence
{
    public class ReadDb : BaseDb<ReadContext>,IReadDb
    {
        public ReadDb(string dbName, bool isPersistent, bool inMemory) 
            : base(dbName, isPersistent, inMemory) { }
    }
}