using System;

namespace LNF.DataAccess
{
    public interface IUnitOfWork : IDisposable
    {
        void Commit();
        void Rollback();
        void Close();
    }
}
