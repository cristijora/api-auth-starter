using System;

namespace DataAcces.Contracts
{
    public interface IUnitOfWork:IDisposable
    {
        void Begin();
        void Commit();
        void Rollback();
        IDataContext Session { get; }
        Guid SessionId { get; }
    }
}