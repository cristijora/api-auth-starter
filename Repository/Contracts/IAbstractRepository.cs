using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using EntityModel.Contracts;

namespace Repository.Contracts
{
    public interface IAbstractRepository<T> where T : IGuidEntity
    {

        T GetById(Guid id, params Expression<Func<T, object>>[] includeProperties);

        IEnumerable<T> GetAll(params Expression<Func<T, object>>[] includeProperties);

        IEnumerable<T> GetWhere(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includeProperties);

        IEnumerable<T> GetAll(IQueryFilter<T> filter, params Expression<Func<T, object>>[] includeProperties);

        IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);

        T SingleBy(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);

        T Save(T entity);

        T Update(T entity);

        void Delete(T entity);

        void DeleteMany(List<T> entities);

        void DeleteAll();

        int Count(Expression<Func<T, bool>> predicate);

        void RemoveListOfChild<T2>(List<T2> childEntities) where T2 : class, IGuidEntity;

        void ClearDatabase();

        void MarkDeleted(T entity);

        void UpdateMany(List<T> entities);
        void SaveMany(List<T> entities);
    }
}
