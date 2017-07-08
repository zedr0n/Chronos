using System;

namespace Chronos.Infrastructure.Projections
{
    public interface IProjectorRepository
    {
        IProjector Get(Type t);
    }
}