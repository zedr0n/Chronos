using Chronos.Persistence;
using SimpleInjector;

namespace Chronos.CrossCuttingConcerns.DependencyInjection
{
    public interface ICompositionRoot
    {
        ICompositionRootRead ReadWith();
        ICompositionRootWrite WriteWith();
        void ComposeApplication(Container container);
    }

    public interface ICompositionRootRead
    {
        ICompositionRootRead InMemory();
        ICompositionRootRead Persistent();
        ICompositionRoot Database(string dbName);
    }

    public interface ICompositionRootWrite
    {
        ICompositionRootWrite InMemory();
        ICompositionRootWrite Persistent();
        ICompositionRoot Database(string dbName);
    } 
}