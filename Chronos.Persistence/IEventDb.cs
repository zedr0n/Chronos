using Chronos.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Chronos.Persistence
{
    public interface IDb
    {
        void Init();
        DbContext GetContext();
        void Clear();
    }

    public interface IDb<T> : IDb where T : DbContext
    {
        
    }
    
    public interface IEventDb : IDb<EventContext>
    {
        
    }
}