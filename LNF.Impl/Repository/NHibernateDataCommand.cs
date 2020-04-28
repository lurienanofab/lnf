using LNF.DataAccess;
using LNF.Impl.DataAccess;
using LNF.Repository;
using System.Data;

namespace LNF.Impl.Repository
{
    public class NHibernateDataCommand : DataCommandBase
    {
        private readonly NHibernate.ISession _session;

        public NHibernateDataCommand(NHibernate.ISession session, CommandType type) : base(type)
        {
            _session = session;
        }

        protected override IUnitOfWorkAdapter GetAdapter()
        {
            return new NHibernateUnitOfWorkAdapter(_session);
        }
    }
}
