using LNF.Repository;
using NHibernate;
using System.Data;
using System.Data.Common;

namespace LNF.Impl.DataAccess
{
    public class NHibernateUnitOfWorkAdapter : UnitOfWorkAdapter
    {
        private ISession session;

        internal NHibernateUnitOfWorkAdapter(ISession session)
        {
            this.session = session;
            SelectCommand = GetCommand();
            InsertCommand = GetCommand();
            UpdateCommand = GetCommand();
            DeleteCommand = GetCommand();
        }

        private DbCommand GetCommand()
        {
            var result = session.Connection.CreateCommand();
            result.CommandType = CommandType.StoredProcedure;
            var tx = session.GetCurrentTransaction();
            if (tx != null) tx.Enlist(result);
            return result;
        }
    }
}
