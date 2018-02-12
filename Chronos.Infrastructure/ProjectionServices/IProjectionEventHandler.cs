using Chronos.Infrastructure.Interfaces;

namespace Chronos.Infrastructure.ProjectionServices
{
    public interface IProjectionEventHandler<T> where T : IReadModel
    {
        bool Handle(T readModel, IEvent e);
    }

    public class BaseProjectionEventHandler<T> : IProjectionEventHandler<T> where T : IReadModel
    {
        public virtual bool Handle(T readModel, IEvent e)
        {
            return readModel.When(e);
        }
    }
}