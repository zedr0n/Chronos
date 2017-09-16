using System;

namespace Chronos.Infrastructure.Commands
{
    public class ParseJsonRequestCommand : CommandBase
    {
        public Guid RequestId { get; set; }    
    }
}