using System;

namespace Chronos.Infrastructure
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SourceAttribute : Attribute
    {
        private readonly Type _sourceAggregate;
        public string SerializableName => _sourceAggregate.SerializableName(); 

        public SourceAttribute(Type sourceAggregate)
        {
            _sourceAggregate = sourceAggregate;
        }
    }
    
    /// <inheritdoc />
    /// <summary>
    /// If set, read model will be updated from first event in the stream
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ResetAttribute : Attribute
    {
        
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class MemoryProxyAttribute : Attribute
    {
        
    }
}