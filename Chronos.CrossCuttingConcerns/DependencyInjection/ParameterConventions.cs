using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using SimpleInjector;
using SimpleInjector.Advanced;

namespace Chronos.CrossCuttingConcerns.DependencyInjection
{
    public interface IParameterConvention
    {
        bool CanResolve(InjectionTargetInfo target);
        Expression BuildExpression(InjectionConsumerInfo consumer);
    }

    public static class ParameterConventionExtensions
    {
        public static void RegisterParameterConventions(this ContainerOptions options,IEnumerable<IParameterConvention> conventions)
        {
            foreach(var c in conventions)
                RegisterParameterConvention(options,c);
        }
        public static void RegisterParameterConvention(this ContainerOptions options,
            IParameterConvention convention)
        {
            options.DependencyInjectionBehavior = new ConventionDependencyInjectionBehavior(
                options.DependencyInjectionBehavior, convention, options.Container);
        }

        private class ConventionDependencyInjectionBehavior : IDependencyInjectionBehavior
        {
            private readonly IDependencyInjectionBehavior _decoratee;
            private readonly IParameterConvention _convention;
            private readonly Container _container;

            internal ConventionDependencyInjectionBehavior(
                IDependencyInjectionBehavior decoratee, IParameterConvention convention, Container container)
            {
                _decoratee = decoratee;
                _convention = convention;
                _container = container;
            }

            [DebuggerStepThrough]
            public void Verify(InjectionConsumerInfo consumer)
            {
                if (!_convention.CanResolve(consumer.Target))
                {
                    _decoratee.Verify(consumer);
                }
            }

            public InstanceProducer GetInstanceProducer(InjectionConsumerInfo consumer, bool throwOnFailure)
            {
                if (!_convention.CanResolve(consumer.Target))
                {
                    return _decoratee.GetInstanceProducer(consumer, throwOnFailure);
                }

                return InstanceProducer.FromExpression(
                    consumer.Target.TargetType,
                    _convention.BuildExpression(consumer),
                    _container);
            }
        }

    }

    public class PersistentDbConvention : IParameterConvention
    {
        private readonly bool _persistentDb;

        public PersistentDbConvention(bool persistentDb)
        {
            _persistentDb = persistentDb;
        }

        public bool CanResolve(InjectionTargetInfo target) => target.TargetType == typeof(bool) && target.Name == "isPersistent";

        public Expression BuildExpression(InjectionConsumerInfo consumer)
        {
            return Expression.Constant(_persistentDb, typeof(bool));
        }
    }

    public class InMemoryConvention : IParameterConvention
    {
        private readonly bool _inMemory;

        public InMemoryConvention(bool inMemory)
        {
            _inMemory = inMemory;
        }

        public bool CanResolve(InjectionTargetInfo target) => target.TargetType == typeof(bool) && target.Name == "inMemory";

        public Expression BuildExpression(InjectionConsumerInfo consumer)
        {
            return Expression.Constant(_inMemory, typeof(bool));
        }
    }

    public class DbNameStringConvention : IParameterConvention
    {
        private readonly string _dbName;

        public DbNameStringConvention(string dbName)
        {
            _dbName = dbName;
        }

        public bool CanResolve(InjectionTargetInfo target) => target.TargetType == typeof(string) && target.Name == "dbName";

        public Expression BuildExpression(InjectionConsumerInfo consumer)
        {
            return Expression.Constant(_dbName, typeof(string));
        }
    }
}