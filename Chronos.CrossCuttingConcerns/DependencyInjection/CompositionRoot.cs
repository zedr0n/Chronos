using Chronos.Core.Accounts.Commands;
using Chronos.Core.Accounts.Projections;
using Chronos.Core.Accounts.Queries;
using Chronos.Core.Sagas;
using Chronos.Core.Transactions.Commands;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;
using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Projections.New;
using Chronos.Infrastructure.Queries;
using Chronos.Infrastructure.Sagas;
using Chronos.Persistence;
using Chronos.Persistence.Serialization;
using NodaTime;
using SimpleInjector;

namespace Chronos.CrossCuttingConcerns.DependencyInjection
{
    public class CompositionRoot : ICompositionRoot, ICompositionRootWithDatabase
    {

        private DbConfiguration Database { get; set; } = new DbConfiguration("Default");

        public ICompositionRootWithDatabase WithDatabase(string dbName)
        {
            var database = new DbConfiguration(dbName);
            return new CompositionRoot {Database = database};
        }

        public ICompositionRootWithDatabase InMemory()
        {
            Database = Database.InMemory();
            return this;
        }

        public ICompositionRootWithDatabase Persistent()
        {
            Database = Database.Persistent();
            return this;
        }

        public virtual void ComposeApplication(Container container)
        {
            container.Options.RegisterParameterConventions(Database.Conventions);
            
            // register infrastructure
            container.Register<IEventBus,EventBus>(Lifestyle.Singleton);
            container.Register<ISerializer,JsonTextSerializer>(Lifestyle.Singleton);
            container.Register<IEventDb, EventDb>(Lifestyle.Singleton);
            container.Register<IEventStoreConnection,SqlStoreConnection>(Lifestyle.Singleton);
            container.Register<IDomainRepository,EventStoreDomainRepository>(Lifestyle.Singleton);
            container.Register<ISagaRepository,EventStoreSagaRepository>(Lifestyle.Singleton);
            container.Register<ITimeline,Timeline>(Lifestyle.Singleton);
            container.Register<ITimeNavigator, TimeNavigator>(Lifestyle.Singleton);
            container.Register<ICommandRegistry,CommandRegistry>(Lifestyle.Singleton);
            container.Register<ICommandBus, CommandBus>(Lifestyle.Singleton);
            container.Register<ISagaManager,SagaManager>(Lifestyle.Singleton);
            container.Register<ITimerService,TimerService>(Lifestyle.Singleton);
            container.Register<IClock,HighPrecisionClock>(Lifestyle.Singleton);
            container.Register<IReadRepository, ReadRepository>(Lifestyle.Singleton);
            container.Register<IStateWriter,StateWriter>(Lifestyle.Singleton);
            container.Register<IEventSerializer,EventSerializer>(Lifestyle.Singleton);
            container.Register(
                () => container.GetInstance<IEventStoreConnection>().Subscriptions, Lifestyle.Singleton);
            container.Register(typeof(IBaseProjectionExpression<>),typeof(ProjectionExpression<>));
            container.Register<IProjectionManager,ProjectionManager>(Lifestyle.Singleton);

            container.Register(typeof(ICommandHandler<>),new[] {
                typeof(CreateAccountHandler),
                typeof(ChangeAccountHandler),
                typeof(DepositCashHandler),
                typeof(DepositAssetHandler),
                typeof(WithdrawCashHandler),
                typeof(CreatePurchaseHandler),
                typeof(ScheduleCommandHandler),
                typeof(CreateCashTransferHandler),
                typeof(RequestTimeoutHandler)
            } ,Lifestyle.Singleton);
            //container.Register<ICommandHandler<CreateAccountCommand>, CreateAccountHandler>(Lifestyle.Singleton);
            //container.Register<ICommandHandler<ChangeAccountCommand>, ChangeAccountHandler>(Lifestyle.Singleton);
            //container.Register<ICommandHandler<DepositCashCommand>, DepositCash>(Lifestyle.Singleton);
            //container.Register<ICommandHandler<WithdrawCashCommand>,WithdrawCashHandler>(Lifestyle.Singleton);
            //container.Register<ICommandHandler<CreatePurchaseCommand>, CreatePurchaseHandler>(Lifestyle.Singleton);
            //container.Register<ICommandHandler<ScheduleCommand>,ScheduleCommandHandler>(Lifestyle.Singleton);
            container.Register<IQueryHandler<GetAccountInfo,AccountInfo>,GetAccountInfoHandler>(Lifestyle.Singleton);
            container.Register<IQueryHandler<GetTotalMovement,TotalMovement>,GetTotalMovementHandler>(Lifestyle.Singleton);
            container.Register(typeof(IQueryHandler<HistoricalQuery<GetAccountInfo,AccountInfo>,AccountInfo>),
                typeof(HistoricalQueryHandler<GetAccountInfo,AccountInfo>), Lifestyle.Singleton);
        }
    }
}