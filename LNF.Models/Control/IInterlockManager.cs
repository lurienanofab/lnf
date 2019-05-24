using System.Collections.Generic;

namespace LNF.Models.Control
{
    public interface IInterlockManager
    {
        IEnumerable<IToolStatus> GetToolStatus();

        BlockStateItem GetBlockState(int blockId);

        ActionInstanceItem GetAllActionInstances();
    }
}
