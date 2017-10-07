namespace Chronos.Infrastructure
{
    public struct StreamRequest
    {
        public StreamDetails Stream { get; }
        public int Version { get; }

        public StreamRequest(StreamDetails stream, int version)
        {
            Stream = stream;
            Version = version;
        }
    }
}