using System.Data;

namespace LNF.Repository
{
    public abstract class ManagerBase : IManager
    {
        public ISession Session { get; }
        public DataCommandBase Command(CommandType type = CommandType.StoredProcedure) => DA.Command(type);

        public ManagerBase(ISession session)
        {
            Session = session;
        }
    }
}
