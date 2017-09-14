using System;

namespace Chronos.Infrastructure.Commands
{
    public class ParseJsonRequestCommand<T,TCommand> : CommandBase
        where T : class 
        where TCommand : class
    {
        public Guid RequestId { get; set; }    
    }
}