using System;

namespace Chronos.Infrastructure.Exceptions
{
    public class ConcurrencyException : InvalidOperationException
    {
        public ConcurrencyException(string message)    
            : base(message) {}
    }
}