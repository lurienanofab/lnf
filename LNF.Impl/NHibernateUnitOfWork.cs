using LNF.Repository;
using NHibernate;
using NHibernate.Context;
using System;
using System.Data;

namespace LNF.Impl
{
    public class NHibernateUnitOfWork<TContext> : IUnitOfWork where TContext : ICurrentSessionContext
    {
        private ITransaction _transaction;

        public NHibernateUnitOfWork()
        {
            SessionManager<TContext>.Current.OpenSession();
            _transaction = SessionManager<TContext>.Current.Session.BeginTransaction(IsolationLevel.ReadCommitted);
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
            SessionManager<TContext>.Current.CloseSession();
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
