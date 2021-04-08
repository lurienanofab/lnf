using LNF.DataAccess;
using System;
using System.Data;

namespace LNF.Repository
{
    [Obsolete("Use LNF.Impl.Repository instead.")]
    public abstract class ManagerBase : IManager
    {
        public IProvider Provider { get; }
        public ISession Session => throw new NotImplementedException();
        public IDataCommand Command(CommandType type = CommandType.StoredProcedure) => DataCommand.Create(type);

        public ManagerBase(IProvider provider)
        {
            Provider = provider;
        }
    }
}
