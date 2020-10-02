using System.Collections.Generic;

namespace LNF.Control
{
    public interface IControlService
    {
        BlockResponse GetBlockState(int blockId);
        bool GetPointState(int pointId);
        PointResponse SetPointState(int pointId, bool state, uint duration);
        PointResponse Cancel(int pointId);
        IEnumerable<IToolStatus> GetToolStatus();
        IBlockConfig GetBlockConfig(int blockId, byte modPosition);
        IEnumerable<IActionInstance> GetActionInstances();
        IEnumerable<IActionInstance> GetActionInstances(string actionName);
        IEnumerable<IActionInstance> GetActionInstances(string actionName, int index);
        IActionInstance GetActionInstance(ActionType action, int actionId);
        IActionInstance GetActionInstance(ActionType action, IPoint point);
        IBlock GetBlock(int blockId);
        IEnumerable<IBlock> GetBlocks();
        IPoint GetPoint(int pointId);
        IControlAction GetControlAction(string actionName);
        IEnumerable<IControlAuthorization> GetControlAuthorizations();
    }
}
