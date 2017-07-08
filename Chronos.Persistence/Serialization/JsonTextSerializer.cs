using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Chronos.Persistence.Serialization
{
    public class JsonTextSerializer : ISerializer
    {
        private readonly JsonSerializer _serializer;

        public JsonTextSerializer()
        {
            _serializer = JsonSerializer.Create(new JsonSerializerSettings
            {
                // Allows deserializing to the actual runtime type
                TypeNameHandling = TypeNameHandling.All,
                // In a version resilient way
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
                Converters = new List<JsonConverter> { new InstantJsonConverter() },
                DateParseHandling = DateParseHandling.None
                //TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple
            });

        }

        public void Serialize<T>(TextWriter writer, T obj)
        {
            var jsonWriter = new JsonTextWriter(writer) { Formatting = Formatting.Indented };

            _serializer.Serialize(jsonWriter, obj);

            // We don't close the stream as it's owned by the message.
            writer.Flush();

        }

        public T Deserialize<T>(TextReader reader)
        {
            var jsonReader = new JsonTextReader(reader);

            try
            {
                return (T)_serializer.Deserialize(jsonReader);
            }
            catch (JsonSerializationException e)
            {
                // Wrap in a standard .NET exception.
                throw new SerializationException(e.Message, e);
            }

        }
    }
}