using System;

namespace Chronos.Infrastructure
{
    public struct StreamDetails
    {
        public string Name { get; }
        public Type SourceType { get; }

        public StreamDetails(string name, Type sourceType = null)
        {
            Name = name;
            SourceType = sourceType;
        }
    }
}