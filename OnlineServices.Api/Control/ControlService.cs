using LNF.Control;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Control
{
    public class ControlService : ApiClient, IControlService
    {
        public BlockResponse GetBlockState(IBlock block)
        {
            throw new NotImplementedException();
        }

        public PointResponse SetPointState(IPoint point, bool state, uint duration)
        {
            throw new NotImplementedException();
        }

        public PointResponse Cancel(IPoint point)
        {
            throw new NotImplementedException();
        }

        public BlockResponse GetBlockState(int blockId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IToolStatus> GetToolStatus()
        {
            return Get<List<ToolStatusItem>>("webapi/control/status");
        }

        public IEnumerable<IActionInstance> GetActionInstances()
        {
            throw new NotImplementedException();
        }

        public bool GetPointState(int pointId)
        {
            throw new NotImplementedException();
        }

        public PointResponse SetPointState(int pointId, bool state, uint duration)
        {
            throw new NotImplementedException();
        }

        public PointResponse Cancel(int pointId)
        {
            throw new NotImplementedException();
        }

        public IActionInstance GetActionInstance(ActionType action, int actionId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IBlock> GetBlocks()
        {
            throw new NotImplementedException();
        }

        public IPoint GetPoint(int pointId)
        {
            throw new NotImplementedException();
        }

        public IBlockConfig GetBlockConfig(int blockId, byte modPosition)
        {
            throw new NotImplementedException();
        }

        public IActionInstance GetActionInstance(ActionType action, IPoint point)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IActionInstance> GetActionInstances(string actionName)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IActionInstance> GetActionInstances(string actionName, int index)
        {
            throw new NotImplementedException();
        }

        public IControlAction GetControlAction(string actionName)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IControlAuthorization> GetControlAuthorizations()
        {
            throw new NotImplementedException();
        }
    }
}
