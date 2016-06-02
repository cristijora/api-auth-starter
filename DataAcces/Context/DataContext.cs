
using System;
using System.Data.Entity;
using DataAcces.Utils;
using Microsoft.AspNet.Identity.EntityFramework;
using DataAcces.Context;
using DataAcces.Contracts;
using EntityModel.Contracts;
using EntityModel.Models;

namespace DataAcces.Context
{
    public class DataContext : IdentityDbContext<User>,IDataContext
    {
        private readonly Guid _contextId;
        private readonly DataBaseModelDefinitions _databaseModelDefinitions;

        public DbSet<TEntity> Set<TEntity>() where TEntity:class
        {
            return base.Set<TEntity>();
        }
        public Database ContextDatabase
        {
            get
            {
                return Database;
            }
        }

        public Guid SessionId
        {
            get { return _contextId; }
        }
        public DataContext() : base("DataContext", throwIfV1Schema: false)
        {
            _databaseModelDefinitions = new DataBaseModelDefinitions();
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            _databaseModelDefinitions.DefineConfigurations(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        public static DataContext Create()
        {
            return new DataContext();
        }

        /// <summary>
        /// Save changes to the database
        /// </summary>
        /// <returns></returns>
        public override int SaveChanges()
        {
            return ExceptionHandler.ExecuteDatabaseSave(base.SaveChanges);
        }
    }
}