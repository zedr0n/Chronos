using SimpleInjector;

namespace Chronos.CrossCuttingConcerns.DependencyInjection
{
    public interface ICompositionRoot
    {
        ICompositionRootWithDatabase WithDatabase(string dbName);
        void ComposeApplication(Container container);
    }
    
    public interface ICompositionRootWithDatabase
    {
        ICompositionRootWithDatabase InMemory();
        ICompositionRootWithDatabase Persistent();
        void ComposeApplication(Container container);
    }
}