﻿using System.Linq;
using System.Reflection;
using Chronos.Core.Account.Commands;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;
using Chronos.Infrastructure.Events;
using Chronos.Persistence;
using Chronos.Persistence.Serialization;
using SimpleInjector;

namespace Chronos.CrossCuttingConcerns.DependencyInjection
{
    public class CompositionRoot
    {
        public virtual void ComposeApplication(Container container, string dbName,bool isPersistentDb)
        {
            container.Options.RegisterParameterConvention(new DbNameStringConvention(dbName + ".db"));
            container.Options.RegisterParameterConvention(new PersistentDbConvention(isPersistentDb));
            
            // register infrastructure
            container.Register<IEventBus,EventBus>(Lifestyle.Singleton);
            container.Register<ISerializer,JsonTextSerializer>(Lifestyle.Singleton);
            container.Register<IEventDb, EventDb>(Lifestyle.Singleton);
            container.Register<IEventStoreConnection,SqlStoreConnection>(Lifestyle.Singleton);
            container.Register<IDomainRepository,EventStoreDomainRepository>(Lifestyle.Singleton);

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

        }
    }
}