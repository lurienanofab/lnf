using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using NHibernate;
using LNF.Repository;

namespace LNF.Impl
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
            var result = session.Connection.CreateCommand() as DbCommand;
            result.CommandType = CommandType.StoredProcedure;
            session.Transaction.Enlist(result);
            return result;
        }
    }
}
