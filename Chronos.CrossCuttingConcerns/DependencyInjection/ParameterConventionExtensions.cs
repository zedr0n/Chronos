﻿using System.Collections.Generic;
using System.Diagnostics;
using Chronos.Persistence;
using SimpleInjector;
using SimpleInjector.Advanced;

namespace Chronos.CrossCuttingConcerns.DependencyInjection
{
    public static class ParameterConventionExtensions
    {
        public static void RegisterParameterConventions(this ContainerOptions options,IEnumerable<IParameterConvention> conventions)
        {
            if (conventions == null)
                return;
            foreach(var c in conventions)
                RegisterParameterConvention(options,c);
        }
        public static void RegisterParameterConvention(this ContainerOptions options,
            IParameterConvention convention)
        {
            options.DependencyInjectionBehavior = new ConventionDependencyInjectionBehavior(
                options.DependencyInjectionBehavior, convention, options.Container);
        }

        private struct ConventionDependencyInjectionBehavior : IDependencyInjectionBehavior
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
                if (!_convention.CanResolve(consumer.Target) || !_convention.Handles(consumer))
                    return _decoratee.GetInstanceProducer(consumer, throwOnFailure);

                return InstanceProducer.FromExpression(
                    consumer.Target.TargetType,
                    _convention.BuildExpression(consumer),
                    _container);
            }
        }

    }
}