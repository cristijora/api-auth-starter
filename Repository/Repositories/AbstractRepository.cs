using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using DataAcces.Contracts;
using EntityModel.Contracts;
using Repository.Contracts;
using Repository.Utils;

namespace Repository.Repositories
{
    public class AbstractRepository<T> : IAbstractRepository<T> where T : class, IGuidEntity
    {
       
        protected readonly IDataContext UnitOfWork;
        internal readonly DbSet<T> DbSet;

        protected AbstractRepository(IDataContext unitOfWork)
        {
            UnitOfWork = unitOfWork;
            DbSet = UnitOfWork.Set<T>();
        }

        public T GetById(Guid id, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> entityQuery = DbSet.AsQueryable();
            entityQuery = includeProperties.Aggregate(entityQuery, (current, includeProperty) => current.Include(includeProperty));

            return entityQuery.FirstOrDefault(t => t.Id == id);
        }

        public IEnumerable<T> GetAll(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> entityQuery = DbSet.AsQueryable();
            entityQuery = includeProperties.Aggregate(entityQuery, (current, includeProperty) => current.Include(includeProperty));

            return entityQuery.ToList();
        }

        public IEnumerable<T> GetWhere(Expression<Func<T, bool>> filter,
            params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> entityQuery = DbSet.AsQueryable().Where(filter);
            entityQuery = includeProperties.Aggregate(entityQuery, (current, includeProperty) => current.Include(includeProperty));

            return entityQuery.ToList();
        }

        public IEnumerable<T> GetAll(IQueryFilter<T> filter, params Expression<Func<T, object>>[] includeProperties)
        {
            var entityQuery = filter.FilterQueryable(DbSet.AsQueryable());
            entityQuery = includeProperties.Aggregate(entityQuery, (current, includeProperty) => current.Include(includeProperty));

            return entityQuery.ToList();
        }

        public IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> entityQuery = DbSet.Where(predicate);
            entityQuery = includeProperties.Aggregate(entityQuery, (current, includeProperty) => current.Include(includeProperty));
            return entityQuery;
        }

        public T SingleBy(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            List<T> elements = FindBy(predicate, includeProperties).ToList();

            if (elements.Any())
            {
                if (elements.Count() > 1)
                    throw new InvalidOperationException("More than one element in the sequence");

                return elements.First();
            }

            return null;
        }

        public T Save(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            T updatedEntity = DbSet.Add(entity);
            UnitOfWork.SaveChanges();

            return updatedEntity;
        }

        public void SaveMany(List<T> entities)
        {
            if (entities == null || !entities.Any())
                throw new ArgumentNullException("entities");

            foreach (var entity in entities)
            {
                DbSet.Add(entity);
            }

            UnitOfWork.SaveChanges();
        }

        public void UpdateMany(List<T> entities)
        {
            if (!entities.Any())
                throw new ArgumentNullException("entities");

            foreach (var entity in entities)
            {
                EnsureAttachedEf(entity).State = EntityState.Modified;
                var orig = DbSet.Find(entity.Id);
                if (orig != null)
                {
                    UnitOfWork.Entry(entity).CurrentValues.SetValues(entity);
                }
            }

            UnitOfWork.SaveChanges();
        }

        public T Update(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            //T updatedEntity = DbSet.Attach(entity);
            EnsureAttachedEf(entity).State = EntityState.Modified;
            var orig = DbSet.Find(entity.Id);
            if (orig != null)
            {
                UnitOfWork.Entry(entity).CurrentValues.SetValues(entity);
            }

            UnitOfWork.SaveChanges();

            return entity;
        }

        public void ClearDatabase()
        {
            //TODO implement this to clear database
        }

        public virtual void Delete(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            EnsureAttachedEf(entity);
            DbSet.Remove(entity);
            UnitOfWork.SaveChanges();
        }

        public virtual void DeleteMany(List<T> entities)
        {
            if (entities == null || !entities.Any())
                throw new ArgumentNullException("entities");

            foreach (var entity in entities)
            {
                EnsureAttachedEf(entity);
                DbSet.Remove(entity);
            }

            UnitOfWork.SaveChanges();
        }

        public virtual void DeleteAll()
        {
            var entities = GetAll();
            foreach (var entity in entities)
            {
                DbSet.Remove(entity);
            }

            UnitOfWork.SaveChanges();
        }

        public virtual void DeleteWhere(Expression<Func<T, bool>> filter)
        {
            var entities = GetWhere(filter);
            foreach (var entity in entities)
            {
                DbSet.Remove(entity);
            }

            UnitOfWork.SaveChanges();
        }

        private IQueryable<T> FindByQueryable(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            var entityQuery = DbSet.Where(predicate);
            entityQuery = includeProperties.Aggregate(entityQuery, (current, includeProperty) => current.Include(includeProperty));
            return entityQuery;
        }


        public IQueryable<T> GetLazy(Expression<Func<T, bool>> predicate = null, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = DbSet;

            if (predicate != null)
            {
                query = FindByQueryable(predicate, includeProperties);
            }

            return query;
        }

        public PagedData<T> GetPagedData(int pageNo, int rows, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy, Expression<Func<T, bool>> predicate = null, params Expression<Func<T, object>>[] includeProperties)
        {
            //Add back in order by to optimize perforamnce on large DS
            var startAt = (pageNo * rows) - rows;

            var retrieved = GetLazy(predicate, includeProperties); //Get an instance of IQueryable to use for the class
            var totalRows = retrieved.Count(); //Count before paging occurs
            var totalPages = (int)Math.Ceiling(totalRows / (double)rows);

            if (pageNo > totalPages) pageNo = totalPages;

            var filteredResults = ApplyPaging(retrieved, orderBy, startAt, rows);

            var ret = new PagedData<T>
            {
                PageNo = pageNo,
                TotalPages = totalPages,
                TotalRows = totalRows,
                Data = filteredResults
            };

            return ret;
        }

        private static IQueryable<T> ApplyPaging(IQueryable<T> query, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, int skip = 0, int take = 0)
        {
            if (orderBy != null)
            {
                return take != 0 ? orderBy(query).Skip(skip).Take(take) : orderBy(query).Skip(skip);
            }

            return query;
        }

        public int Count(Expression<Func<T, bool>> predicate)
        {
            return DbSet.Where(predicate).Count();
        }

        protected DbEntityEntry<T> EnsureAttachedEf(T entity)
        {
            if (UnitOfWork.Entry(entity).State == EntityState.Detached)
                DbSet.Attach(entity);

            return UnitOfWork.Entry(entity);
        }

        public void MarkDeleted(T entity)
        {
            if (UnitOfWork.Entry(entity).State == EntityState.Detached)
                DbSet.Attach(entity);

            UnitOfWork.Entry(entity).State = EntityState.Deleted;
        }

        public void RemoveListOfChild<T2>(List<T2> _listOfChildEntities) where T2 : class, IGuidEntity
        {
            foreach (T2 child in _listOfChildEntities)
            {
                if (UnitOfWork.Entry(child).State == EntityState.Detached)
                    UnitOfWork.Set<T2>().Attach(child);
                UnitOfWork.Set<T2>().Remove(child);
            }
        }
    }
}
