using System.Threading;
using Chronos.Infrastructure;

namespace Chronos.Console
{
    public class BagQueryListener : ChronosBaseListener
    {
        private readonly IReadRepository _readRepository;

        public BagQueryListener(IReadRepository readRepository)
        {
            _readRepository = readRepository;
        }

        public override void EnterBags(ChronosParser.BagsContext context)
        {
            base.EnterBags(context);
        }
    }
}