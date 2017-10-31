using System;
using System.Numerics;
using Newtonsoft.Json;
using NodaTime;

namespace Chronos.Persistence.Serialization
{
    public class DurationJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Duration);
        }
        
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((Duration) value).ToBigIntegerNanoseconds());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType == typeof(Duration))
                return Duration.FromNanoseconds((Int64) reader.Value);
                //return InstantPattern.ExtendedIso.Parse((string) reader.Value).Value;

            return reader.Value;
        }
    }
}