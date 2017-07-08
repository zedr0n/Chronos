using System;
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
}