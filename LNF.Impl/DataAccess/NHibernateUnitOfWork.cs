using LNF.CommonTools;
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
            IsolationLevel isolationLevel;
            //isolationLevel = IsolationLevel.ReadUncommitted;
            isolationLevel = IsolationLevel.ReadCommitted;
            _transaction = SessionManager.Session.BeginTransaction(isolationLevel);
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
            catch (Exception ex1)
            {
                // The Microsoft docs for Rolling backs transactions has a nested try/catch around the rollback call. Without this we can end up with zombie transaction errors.
                // https://docs.microsoft.com/en-us/dotnet/api/system.data.sqlclient.sqltransaction.rollback?view=dotnet-plat-ext-3.1&viewFallbackFrom=netstandard-2.1

                var ex = new TransactionCommitException()
                {
                    CommitException = ex1,
                    SelectStatement = "?",
                    InsertStatement = "?",
                    DeleteStatement = "?",
                    UpdateStatement = "?"
                };

                try
                {
                    Rollback();
                }
                catch (Exception ex2)
                {
                    ex.RollbackException = ex2;
                }

                throw ex;
            }
            finally
            {
                Close();
            }
        }
    }
}
