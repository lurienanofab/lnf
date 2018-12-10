using System.Data;

namespace LNF.Repository
{
    public abstract class ManagerBase : IManager
    {
        public ISession Session { get; }
        public DataCommand Command(CommandType type = CommandType.StoredProcedure) => DataCommand.Create(Session.GetAdapter, type);

        public ManagerBase(ISession session)
        {
            Session = session;
        }
    }
}
