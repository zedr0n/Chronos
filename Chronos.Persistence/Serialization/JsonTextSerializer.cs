using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using NodaTime;
using NodaTime.Text;

namespace Chronos.Persistence.Serialization
{
    public class InstantJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Instant);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(InstantPattern.ExtendedIso.Format((Instant) value));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType == typeof(Instant))
                return InstantPattern.ExtendedIso.Parse((string) reader.Value).Value;

            return reader.Value;
        }
    }

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