using LNF.CommonTools;
using LNF.Repository;
using System.Data;

namespace LNF.Scripting
{
    public class ReadOnlyDataCommand : DataCommand
    {
        public ReadOnlyDataCommand(CommandType type) : base(GetAdapter, type) { }

        private static UnitOfWorkAdapter GetAdapter()
        {
            return SQLDBAccess.Create("cnSselDataReadOnly");
        }
    }
}
