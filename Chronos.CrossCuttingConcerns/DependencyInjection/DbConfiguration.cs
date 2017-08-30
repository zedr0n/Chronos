using System;
using System.Collections.Generic;

namespace Chronos.CrossCuttingConcerns.DependencyInjection
{
    public class DbConfiguration
    {
        private void BuildConventions()
        {
            if( _name == null)
                throw new InvalidOperationException("Configuration invalid");
            
            _conventions = new List<IParameterConvention>
            {
                new DbNameStringConvention(_name + ".db"),
                new InMemoryConvention(_inMemory),
                new PersistentDbConvention(_isPersistent)
            };
        }

        private List<IParameterConvention> _conventions;

        public IEnumerable<IParameterConvention> Conventions
        {
            get
            {
                if (_conventions == null)
                    BuildConventions();
                return _conventions;
            }
        }

        private readonly string _name;
        private bool _inMemory;
        private bool _isPersistent;

        public DbConfiguration() {}

        private DbConfiguration(string name)
        {
            _name = name;
        }

        public DbConfiguration WithName(string name)
        {
            return new DbConfiguration(name)
            {
                _isPersistent = _isPersistent,
                _inMemory = _inMemory
            };
        }
        public DbConfiguration InMemory()
        {
            return new DbConfiguration(_name)
            {
                _isPersistent = false,
                _inMemory = true
            };
        }
        public DbConfiguration Persistent()
        {
            return new DbConfiguration(_name)
            {
                _isPersistent = true
            };
        }
    }
}