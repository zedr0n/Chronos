using System;

namespace Chronos.Infrastructure
{
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