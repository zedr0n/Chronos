using Chronos.Core.Accounts.Commands;
using Chronos.Core.Accounts.Projections;
using Chronos.Core.Accounts.Queries;
using Chronos.Core.Sagas;
using Chronos.Core.Transactions.Commands;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;
using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Projections;
using Chronos.Infrastructure.Queries;
using Chronos.Infrastructure.Sagas;
using Chronos.Persistence;
using Chronos.Persistence.Serialization;
using NodaTime;
using SimpleInjector;

namespace Chronos.CrossCuttingConcerns.DependencyInjection
{
    public class CompositionRoot
    {
        public virtual void ComposeApplication(Container container, string dbName,bool isPersistentDb, bool inMemory)
        {
            container.Options.RegisterParameterConvention(new InMemoryConvention(inMemory));
            container.Options.RegisterParameterConvention(new DbNameStringConvention(dbName + ".db"));
            container.Options.RegisterParameterConvention(new PersistentDbConvention(isPersistentDb));
            
            // register infrastructure
            container.Register<IEventBus,EventBus>(Lifestyle.Singleton);
            container.Register<ISerializer,JsonTextSerializer>(Lifestyle.Singleton);
            container.Register<IEventDb, EventDb>(Lifestyle.Singleton);
            container.Register<IEventStoreConnection,SqlStoreConnection>(Lifestyle.Singleton);
            container.Register<IDomainRepository,EventStoreDomainRepository>(Lifestyle.Singleton);
            container.Register<ISagaRepository,EventStoreSagaRepository>(Lifestyle.Singleton);
            container.Register<IProjectionRepository,ProjectionRepository>(Lifestyle.Singleton);
            container.Register<IProjectionManager,ProjectionManager>(Lifestyle.Singleton);
            //container.Register(typeof(IProjectionWriter<>),typeof(ProjectionWriter<>),Lifestyle.Singleton);
            container.Register<IProjectorRepository,ProjectorRepository>(Lifestyle.Singleton);
            container.Register<ITimeline,Timeline>(Lifestyle.Singleton);
            container.Register<ITimeNavigator, TimeNavigator>(Lifestyle.Singleton);
            container.Register<ICommandRegistry,CommandRegistry>(Lifestyle.Singleton);
            container.Register<ICommandBus, CommandBus>(Lifestyle.Singleton);
            container.Register<ISagaManager,SagaManager>(Lifestyle.Singleton);
            container.Register<ITimerService,TimerService>(Lifestyle.Singleton);
            container.Register<IClock,HighPrecisionClock>(Lifestyle.Singleton);

            // register commands and queries
            /*var handlerRegistrations = Assembly.Load(new AssemblyName(nameof(Chronos.Core)))
                                                .ExportedTypes.Where(t => t.Name.EndsWith("Handler"))
                                                .Select(t => new
                {
                    Service = t.GetTypeInfo().ImplementedInterfaces.First(),
                    Implementation = t
                });

            foreach(var reg in handlerRegistrations)
                container.Register(reg.Service,reg.Implementation,Lifestyle.Singleton);*/

            container.Register<ICommandHandler<CreateAccountCommand>, CreateAccountHandler>(Lifestyle.Singleton);
            container.Register<ICommandHandler<ChangeAccountCommand>, ChangeAccountHandler>(Lifestyle.Singleton);
            container.Register<ICommandHandler<DebitAmountCommand>, DebitAmountHandler>(Lifestyle.Singleton);
            container.Register<ICommandHandler<CreatePurchaseCommand>, CreatePurchaseHandler>(Lifestyle.Singleton);
            container.Register<ICommandHandler<ScheduleCommand>,ScheduleCommandHandler>(Lifestyle.Singleton);
            container.Register<IQueryHandler<GetAccountInfo,AccountInfo>,GetAccountInfoHandler>(Lifestyle.Singleton);

            container.Register<IProjector<AccountInfo>,AccountInfoProjector>(Lifestyle.Singleton);
        }
    }
}