using Microsoft.EntityFrameworkCore;

namespace Chronos.Persistence
{
    public interface IEventDb
    {
        void Init();
        DbContext GetContext();
    }
}