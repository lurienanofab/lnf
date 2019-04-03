using System.Data;

namespace LNF.Repository
{
    public abstract class ManagerBase : IManager
    {
        public IProvider Provider { get; }
        public ISession Session => Provider.DataAccess.Session;
        public DataCommandBase Command(CommandType type = CommandType.StoredProcedure) => DA.Command(type);

        public ManagerBase(IProvider provider)
        {
            Provider = provider;
        }
    }
}
