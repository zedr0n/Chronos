﻿using System;
using System.Linq.Expressions;
using SimpleInjector;

namespace Chronos.CrossCuttingConcerns.DependencyInjection
{
    public interface IParameterConvention
    {
        Type Consumer { get; set; }

        bool Handles(InjectionConsumerInfo consumer);
        bool CanResolve(InjectionTargetInfo target);
        
        Expression BuildExpression(InjectionConsumerInfo consumer);
        
    }
}