using System.Collections.Generic;

namespace Chronos.Infrastructure.Misc
{
    public interface IAnyEnumerable<out T> : IEnumerable<T>
    {
        bool Any();
    }
}