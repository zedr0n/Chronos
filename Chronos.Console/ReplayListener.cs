using Chronos.Infrastructure;
using NodaTime;

namespace Chronos.Console
{
    public class ReplayListener : ChronosBaseListener
    {
        private readonly ITimeNavigator _navigator;

        public ReplayListener(ITimeNavigator navigator)
        {
            _navigator = navigator;
        }

        public override void EnterReplay(ChronosParser.ReplayContext context)
        {
            var date = context.date();
            if (date == null)
                _navigator.Reset();
            else
            {
                var dateString = date.GetText();
                var day = int.Parse(dateString.Substring(0, 2));
                var month = int.Parse(dateString.Substring(2, 2));
                var pastDate = new ZonedDateTime(new LocalDateTime(2017,month,day,0,0), DateTimeZone.Utc,Offset.Zero).ToInstant();
                _navigator.GoTo(pastDate);
            }
            base.EnterReplay(context);
        }
    }
}