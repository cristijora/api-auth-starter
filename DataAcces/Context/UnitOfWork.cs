using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAcces.Contracts;

namespace DataAcces.Context
{
    public class UnitOfWork:IUnitOfWork
    {
        /// <summary>
        /// Set this through the Create transaction to allow committing transactions across multiple saves.
        /// </summary>
        private DbContextTransaction _dbTransaction;
        private bool _thisIdDisposed;
        public Guid SessionId { get; private set; }

        public IDataContext Session { get; private set; }

        public UnitOfWork(IDataContext context)
        {
            Session = context;
            SessionId = Guid.NewGuid();
            System.Diagnostics.Trace.WriteLine(string.Format(" ========== Context FOR TASTIER UOW (Type = {0}) Request := {1}", GetType(), Session.SessionId));
        }

        public void Begin()
        {
            BeginTransaction();
        }

        public void Commit()
        {
            if (_thisIdDisposed)
            {
                throw new InvalidOperationException("Unit of work disposed");
            }

            System.Diagnostics.Trace.WriteLine(string.Format(" ========== Scoped Context FOR COMMIT (Type = {0}) Request := {1}", GetType(), Session.SessionId));//Log this instead

            CommitTransaction();
        }

        public void Rollback()
        {

            if (_thisIdDisposed)
                throw new InvalidOperationException("Unit of work disposed");

            if (_dbTransaction != null)
            {
                RollbackTransaction();
            }
        }

        public void Dispose()
        {
            _thisIdDisposed = true;

            if (_dbTransaction != null)
            {
                _dbTransaction.Dispose();
            }
        }

        #region Transaction Utils

        private void BeginTransaction()
        {
            _dbTransaction = Session.ContextDatabase.BeginTransaction(IsolationLevel.ReadCommitted);
        }

        private void RollbackTransaction()
        {
            if (_dbTransaction != null)
            {
                _dbTransaction.Rollback();
            }

            // Set the change states back - otherwise, previously rolledback changes will be committed later
            var changedEntries = Session.ChangeTracker.Entries().Where(x => x.State != EntityState.Unchanged).ToList();

            // Reset changed values and set to Unchanged state
            foreach (var entry in changedEntries.Where(x => x.State == EntityState.Modified))
            {
                entry.CurrentValues.SetValues(entry.OriginalValues);
                entry.State = EntityState.Unchanged;
            }

            // Detach added entries
            foreach (var entry in changedEntries.Where(x => x.State == EntityState.Added))
            {
                entry.State = EntityState.Detached;
            }

            // Undeleted deleted entries
            foreach (var entry in changedEntries.Where(x => x.State == EntityState.Deleted))
            {
                entry.State = EntityState.Unchanged;
            }
        }

        private void CommitTransaction()
        {
            // single point of failure
            //Session.SaveChanges();
            _dbTransaction.Commit();
        }

        #endregion


    }
}
