namespace Chronos.Infrastructure.Commands
{
    public class TrackJsonCommand<T> : RequestJsonCommand<T>
    {
        public int UpdateInterval { get; set; }
    }
}