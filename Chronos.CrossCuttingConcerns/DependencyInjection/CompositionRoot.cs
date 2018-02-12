using System;
using System.Collections.Generic;
using System.Linq;
using Chronos.Core.Accounts;
using Chronos.Core.Accounts.Commands;
using Chronos.Core.Accounts.Projections;
using Chronos.Core.Accounts.Queries;
using Chronos.Core.Assets;
using Chronos.Core.Assets.Commands;
using Chronos.Core.Assets.Projections;
using Chronos.Core.Assets.Queries;
using Chronos.Core.Net.Parsing;
using Chronos.Core.Net.Parsing.Commands;
using Chronos.Core.Net.Tracking;
using Chronos.Core.Net.Tracking.Commands;
using Chronos.Core.Net.Tracking.Urls;
using Chronos.Core.Nicehash;
using Chronos.Core.Nicehash.Commands;
using Chronos.Core.Nicehash.Projections;
using Chronos.Core.Nicehash.Queries;
using Chronos.Core.Projections;
using Chronos.Core.Sagas;
using Chronos.Core.Scheduling;
using Chronos.Core.Scheduling.Commands;
using Chronos.Core.Transactions;
using Chronos.Core.Transactions.Commands;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;
using Chronos.Infrastructure.Projections;
using Chronos.Infrastructure.ProjectionServices;
using Chronos.Infrastructure.Queries;
using Chronos.Infrastructure.Sagas;
using Chronos.Persistence;
using Chronos.Persistence.Serialization;
using Chronos.Net;
using Chronos.Persistence.Contexts;
using NodaTime;
using SimpleInjector;
using OrderStatus = Chronos.Core.Nicehash.Projections.OrderStatus;

namespace Chronos.CrossCuttingConcerns.DependencyInjection
{
    public static class CompositionExtensions
    {
        public static void RegisterQuery<TQuery, TResult>(this Container container, Type handlerType, Lifestyle lifestyle) 
            where TResult : class, IReadModel, new()
        {
            container.Register(typeof(IQueryHandler<TQuery,TResult>), handlerType,lifestyle);
        }
    }
   
    public class CompositionRoot : ICompositionRoot, ICompositionRootRead, ICompositionRootWrite
    {
        private RootDbConfiguration _readConfiguration; 
        private RootDbConfiguration _writeConfiguration;

        private class RootDbConfiguration 
        {
            public enum Mode
            {
                Read,
                Write
            }
            
            private DbConfiguration _configuration;
            private readonly CompositionRoot _root;
            private readonly Mode _mode;
            
            internal RootDbConfiguration(Mode mode,CompositionRoot root)
            {
                _root = root;
                _mode = mode;
                _configuration = new DbConfiguration();
            }

            internal IEnumerable<IParameterConvention> GetConventions()
            {
                var conventions = _configuration.Conventions.ToList();
                foreach (var parameterConvention in conventions)
                    parameterConvention.Consumer = _mode == Mode.Read ? typeof(IReadDb) : typeof(IEventDb);
                return conventions;
            }

            internal CompositionRoot InMemory()
            {
                _configuration = _configuration.InMemory();
                return _root;
            }

            internal CompositionRoot Persistent()
            {
                _configuration = _configuration.Persistent();
                return _root;
            }

            internal CompositionRoot Database(string dbName)
            {
                _configuration = _configuration.WithName(dbName);
                return _root;
            }
        }
        
        public ICompositionRootRead ReadWith()
        {
            _readConfiguration = new RootDbConfiguration(RootDbConfiguration.Mode.Read,this);
            return this;
        }
        ICompositionRootRead ICompositionRootRead.InMemory() => _readConfiguration.InMemory();
        ICompositionRootRead ICompositionRootRead.Persistent() => _readConfiguration.Persistent();
        ICompositionRoot ICompositionRootRead.Database(string dbName) => _readConfiguration.Database(dbName);
        
        public ICompositionRootWrite WriteWith()
        {
            _writeConfiguration = new RootDbConfiguration(RootDbConfiguration.Mode.Write,this);
            return this;
        }
        ICompositionRootWrite ICompositionRootWrite.InMemory() => _writeConfiguration.InMemory();
        ICompositionRootWrite ICompositionRootWrite.Persistent() => _writeConfiguration.Persistent();
        ICompositionRoot ICompositionRootWrite.Database(string dbName) => _writeConfiguration.Database(dbName);

        public virtual void ComposeApplication(Container container)
        {
            container.Options.RegisterParameterConvention(new NicehashKeyConvention());
            container.Options.RegisterParameterConventions( _readConfiguration?.GetConventions() );
            container.Options.RegisterParameterConventions( _writeConfiguration?.GetConventions() );
            container.Options.RegisterParameterConvention(new AggregateListConvention(new List<Type>
            {
                typeof(Account),
                typeof(Coin),
                typeof(Purchase),
                typeof(Order),
                typeof(Tracker),
                typeof(Bag)
            }) );
            
            // register infrastructure
            container.Register<ISerializer,JsonTextSerializer>(Lifestyle.Singleton);
            container.Register<IEventDb, EventDb>(Lifestyle.Singleton);
            container.Register<IEventStoreConnection,SqlStoreConnection>(Lifestyle.Singleton);
            container.Register<IDomainRepository,EventStoreDomainRepository>(Lifestyle.Singleton);
            container.Register<ISagaRepository,EventStoreSagaRepository>(Lifestyle.Singleton);
            container.Register<ITimeline,Timeline>(Lifestyle.Singleton);
            container.Register<ITimeNavigator, TimeNavigator>(Lifestyle.Singleton);
            container.Register<IScheduler,Scheduler>(Lifestyle.Singleton);
            container.Register<ICommandRegistry,CommandRegistry>(Lifestyle.Singleton);
            container.Register<ICommandBus, CommandBus>(Lifestyle.Singleton);
            container.Register<IQueryProcessor, QueryProcessor>(Lifestyle.Singleton);
            container.Register<IClock,HighPrecisionClock>(Lifestyle.Singleton);
            container.Register<IAggregateFactory,AggregateFactory>(Lifestyle.Singleton);
            container.Register<IJsonConnector,JsonConnector>(Lifestyle.Singleton);
            container.AddRegistration<IEventBus>(Lifestyle.Singleton.CreateRegistration<EventStore>(container));
            container.RegisterConditional<IUrlProvider,OrderUrlProvider>(Lifestyle.Singleton,
                x => x.Consumer.ImplementationType == typeof(TrackOrderHandler));
            container.RegisterConditional<IUrlProvider,CoinUrlProvider>(Lifestyle.Singleton,
                x => x.Consumer.ImplementationType == typeof(TrackCoinHandler));
                
            container.Register<IMemoryReadRepository, ReadRepository>(Lifestyle.Singleton);
            container.Register<IMemoryStateWriter, StateWriter>(Lifestyle.Singleton);
            if (_readConfiguration == null)
            {
                container.Register<IReadRepository, ReadRepository>(Lifestyle.Singleton); 
                container.Register<IStateWriter, StateWriter>(Lifestyle.Singleton);
            }
            else
            {
                container.Register<IReadRepository, SqlReadRepository>(Lifestyle.Singleton); 
                container.Register<IReadDb,ReadDb>(Lifestyle.Singleton);
                container.Register<IStateWriter, DbStateWriter>(Lifestyle.Singleton);
                container.Register<ICommandHandler<ClearDatabaseCommand>,ClearDatabaseHandler>(Lifestyle.Singleton);
            }
            container.Register<IJsonParser,JsonParser>(Lifestyle.Singleton);
            container.Register<IEventSerializer,EventSerializer>(Lifestyle.Singleton);
            container.Register<ICommandSerializer,CommandSerializer>(Lifestyle.Singleton);
            container.Register(typeof(IBaseProjectionExpression<>),typeof(ProjectionExpression<>));
            container.Register<IProjectionManager,ProjectionManager>(Lifestyle.Singleton);
            container.Register<IEventStore, EventStore>(Lifestyle.Singleton);
            container.Register<StreamTracker>(Lifestyle.Singleton);

            container.Register<IProjection<CoinInfo>,StreamPersistentProjection<CoinInfo>>(Lifestyle.Singleton);
            
            container.Register(typeof(ICommandHandler<>),new[] {
                typeof(RequestTimeoutHandler),
                typeof(CancelTimeoutHandler),
                typeof(CreateAccountHandler),
                typeof(ChangeAccountHandler),
                typeof(DepositCashHandler),
                typeof(DepositAssetHandler),
                typeof(WithdrawCashHandler),
                typeof(CreatePurchaseHandler),
                typeof(ScheduleCommandHandler),
                typeof(CreateCashTransferHandler),
                typeof(CreateCoinHandler),
                typeof(UpdateAssetPriceHandler),
                typeof(CreateOrderHandler),
                typeof(TrackOrderHandler),
                typeof(UpdateOrderStatusHandler),
                typeof(RequestJsonHandler),
                typeof(ParseOrderHandler),
                typeof(RequestStopAtHandler),
                typeof(TrackCoinHandler),
                typeof(ParseCoinHandler),
                typeof(StartTrackingHandler),
                typeof(StopTrackingHandler),
                typeof(CreateBagHandler),
                typeof(AddAssetToBagHandler),
                typeof(RemoveAssetFromBagHandler),
                typeof(UpdateAssetChangeHandler)
            } ,Lifestyle.Singleton);
            //container.Register(typeof(IHistoricalCommandHandler<>),typeof(NullCommandHandler<>),Lifestyle.Singleton);
            //container.Register(typeof(IHistoricalCommandHandler<>),typeof(HistoricalCommandHandler<>),Lifestyle.Singleton);
            //container.Register<ICommandHandler<ParseJsonRequestCommand<Orders,UpdateOrderStatusCommand>>,ParseOrderStatusHandler>();

            //container.RegisterDecorator(typeof(ICommandHandler<>),typeof(HistoricalCommandHandler<>),context => 
            //    typeof(IHistoricalCommand).IsAssignableFrom(context.ServiceType.GetGenericArguments().First()));
            container.RegisterDecorator(typeof(ICommandHandler<>),typeof(CommandRecorder<>),Lifestyle.Singleton, context =>
                !context.AppliedDecorators.Any(d => d.IsClosedTypeOf(typeof(CommandRecorder<>)))); 
            
            container.Register<ISagaEventHandler,SagaEventHandler>(Lifestyle.Singleton);
            
            container.Register(typeof(ISagaHandler<>), new[]
            {
                typeof(SchedulerSagaHandler),
                typeof(TransactionSagaHandler),
                typeof(TransferSagaHandler)
            },Lifestyle.Singleton);
            container.Register<ISagaHandler<OrderTrackingSaga>,OrderTrackingSagaHandler>(Lifestyle.Singleton);
            container.Register<ISagaHandler<CoinTrackingSaga>,CoinTrackingSagaHandler>(Lifestyle.Singleton);
            
            container.RegisterQuery<AccountInfoQuery,AccountInfo>(typeof(AccountInfoHandler), Lifestyle.Singleton);
            container.RegisterQuery<CoinInfoQuery,CoinInfo>(typeof(CoinInfoHandler),Lifestyle.Singleton);
            container.RegisterQuery<OrderInfoQuery,OrderInfo>(typeof(OrderInfoHandler),Lifestyle.Singleton);
            container.RegisterQuery<OrderStatusQuery,OrderStatus>(typeof(OrderStatusHandler),Lifestyle.Singleton);
            container.RegisterQuery<TotalMovementQuery,TotalMovement>(typeof(TotalMovementHandler),Lifestyle.Singleton);
            container.RegisterQuery<BagInfoQuery, BagInfo>(typeof(BagInfoHandler),Lifestyle.Singleton);
            container.RegisterQuery<BagHistoryQuery, BagHistory>(typeof(BagHistoryHandler), Lifestyle.Singleton);
            
            container.RegisterQuery<StatsQuery,Stats>(typeof(StatsHandler),Lifestyle.Singleton);
            container.Register(typeof(IHistoricalQueryHandler<AccountInfoQuery,AccountInfo>),
                typeof(HistoricalQueryHandler<AccountInfoQuery,AccountInfo>), Lifestyle.Singleton);
            container.Register(typeof(IHistoricalQueryHandler<TotalMovementQuery,TotalMovement>),
                typeof(HistoricalQueryHandler<TotalMovementQuery,TotalMovement>), Lifestyle.Singleton);
        }

    }
}