using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace DataAcces.Contracts
{
    public interface IDataContext : IObjectContextAdapter, IDisposable
    {
        int SaveChanges();

        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;

        DbChangeTracker ChangeTracker { get; }

        Database ContextDatabase { get; }

        Guid SessionId { get; }
    }
}