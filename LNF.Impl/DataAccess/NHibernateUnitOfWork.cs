using LNF.DataAccess;
using NHibernate;
using System;
using System.Data;

namespace LNF.Impl.DataAccess
{
    public class NHibernateUnitOfWork : IUnitOfWork
    {
        private ITransaction _transaction;

        public ISessionManager SessionManager { get; }

        public NHibernateUnitOfWork(ISessionManager sessionManager)
        {
            SessionManager = sessionManager;
            SessionManager.OpenSession();
            _transaction = SessionManager.Session.BeginTransaction(IsolationLevel.ReadCommitted);
        }

        public void Commit()
        {
            if (_transaction.IsActive)
                _transaction.Commit();
        }

        public void Rollback()
        {
            _transaction.Rollback();
        }

        public void Close()
        {
            _transaction.Dispose();
            SessionManager.CloseSession();
        }

        public void Dispose()
        {
            try
            {
                Commit();
            }
            catch (Exception)
            {
                Rollback();
                throw;
            }
            finally
            {
                Close();
            }
        }
    }
}
