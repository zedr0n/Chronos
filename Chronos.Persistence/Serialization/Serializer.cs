using System.IO;
using System.Xml.Serialization;

namespace Chronos.Persistence.Serialization
{
    public class Serializer : ISerializer
    {
        private readonly XmlSerializerNamespaces _ns;

        public Serializer()
        {
            _ns = new XmlSerializerNamespaces();
            _ns.Add("", "");
        }

        public void Serialize<T>(TextWriter writer, T obj)
        {
            var xmlSerializer = new XmlSerializer(typeof(T));
            xmlSerializer.Serialize(writer, obj, _ns);
            writer.Flush();
        }

        public T Deserialize<T>(TextReader reader)
        {
            var serializer = new XmlSerializer(typeof(T));
            return (T) serializer.Deserialize(reader);
        }
    }
}