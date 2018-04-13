using System;

namespace LNF.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        void Commit();
        void Rollback();
        void Close();
    }
}
