using System.IO;

namespace Chronos.Persistence.Serialization
{
    public interface ISerializer
    {
        void Serialize<T>(TextWriter writer, T obj);

        T Deserialize<T>(TextReader reader);
    }
}