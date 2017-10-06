using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Chronos.Persistence
{
    public static class ContextExtensions
    {
        public static void Clear(this DbContext context)
        {
            var allTypes = context.Model.GetEntityTypes()
                .Select(x => x.ClrType);
            foreach(var type in allTypes)
                context.RemoveRange(context.All(type));
        }

        public static IEnumerable<object> All(this DbContext context, Type entityType)
        {
            return (IEnumerable<object>) typeof(DbContext).GetMethod(nameof(DbContext.Set))
                .MakeGenericMethod(entityType)
                .Invoke(context, new object [] { });
        }
    }
    
    public class Context : DbContext
    {
        //private readonly string _dbName;
        //private readonly bool _inMemory;

        protected Context() { }

        protected Context(DbContextOptions options)
            : base(options)
        {
        }

        public virtual Context WithOptions(DbContextOptions options) => new Context(options);

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //if (optionsBuilder.IsConfigured)
            //    return;
            //if (_inMemory)
            //    optionsBuilder.UseInMemoryDatabase(_dbName);
            //else
                //optionsBuilder.UseSqlite(@"Filename=" + "migration.db");
           
        }
    }
}