using LNF.Models.Control;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Control
{
    public class InterlockManager : ApiClient, IInterlockManager
    {
        public InterlockManager() : base(GetApiBaseUrl()) { }

        public IEnumerable<IToolStatus> GetToolStatus()
        {
            return Get<List<ToolStatusItem>>("webapi/control/status");
        }

        public BlockStateItem GetBlockState(int blockId)
        {
            throw new NotImplementedException();
        }

        public ActionInstanceItem GetAllActionInstances()
        {
            throw new NotImplementedException();
        }
    }
}
