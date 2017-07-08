using System;
using System.Collections.Generic;

namespace Chronos.Infrastructure.Projections
{
    public interface IProjectorRepository
    {
        IProjector Get(Type t);
    }
}