using LNF.Repository;
using System.Data;
using System.Data.Common;

namespace LNF.Impl.DataAccess
{
    public class NHibernateUnitOfWorkAdapter : UnitOfWorkAdapter
    {
        private NHibernate.ISession session;

        internal NHibernateUnitOfWorkAdapter(NHibernate.ISession session)
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
            session.Transaction.Enlist(result);
            return result;
        }
    }
}
