using LNF.Models.Control;
using System;

namespace OnlineServices.Api.Control
{
    public class ControlClient : ApiClient
    {
        public ControlClient() : base(GetApiBaseUrl()) { }

        public object GetAllBlockStates()
        {
            throw new NotImplementedException();
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
