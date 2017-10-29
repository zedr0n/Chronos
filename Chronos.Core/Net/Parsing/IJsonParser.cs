namespace Chronos.Core.Net.Parsing
{
    public interface IJsonParser
    {
        T Parse<T>(string json);
    }
}