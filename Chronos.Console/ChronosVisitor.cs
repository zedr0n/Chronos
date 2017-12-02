using Chronos.Infrastructure.Logging;

namespace Chronos.Console
{
    public class ChronosVisitor : ChronosBaseVisitor<object>
    {
        private readonly IDebugLog _debugLog;    
        
        public ChronosVisitor(IDebugLog debugLog)
        {
            _debugLog = debugLog;
        }
        
        public override object VisitCreateCoin(ChronosParser.CreateCoinContext context)
        {
            var name = context.name();
            _debugLog.WriteLine(name.GetText());
            return base.VisitCreateCoin(context);
        }
    }
}