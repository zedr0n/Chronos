using System.IO;

namespace Chronos.Persistence.Serialization
{
    public interface ISerializer
    {
        void Serialize<T>(TextWriter writer, T obj);

        T Deserialize<T>(TextReader reader);
    }

    /*public static class SerializerExtensions
    {
        /// <summary>
        /// Serializes the given data object as a string.
        /// </summary>
        public static string Serialize<T>(this ISerializer serializer, T data)
        {
            using (var writer = new StringWriter())
            {
                serializer.Serialize(writer, data);
                return writer.ToString();
            }
        }

        /// <summary>
        /// Deserializes the specified string into an object of type <typeparamref name="T"/>.
        /// </summary>
        /// <exception cref="System.InvalidCastException">The deserialized object is not of type <typeparamref name="T"/>.</exception>
        public static T Deserialize<T>(this ISerializer serializer, string serialized)
        {
            using (var reader = new StringReader(serialized))
            {
                return serializer.Deserialize<T>(reader);
            }
        }
    }*/
}