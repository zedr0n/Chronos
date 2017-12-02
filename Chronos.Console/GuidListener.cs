using System;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace Chronos.Console
{
    public class GuidListener : ChronosBaseListener
    {
        public override void EnterNewGuid(ChronosParser.NewGuidContext context)
        {
            var guid = Guid.NewGuid();
            var token = (CommonToken) context.Start;
            token.Text = guid.ToString();
            base.EnterNewGuid(context);
        }
    }
}