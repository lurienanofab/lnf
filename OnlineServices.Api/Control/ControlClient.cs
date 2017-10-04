using LNF.Models.Control;
using System;
using System.Configuration;
using System.Threading.Tasks;

namespace OnlineServices.Api.Control
{
    public class ControlClient : ApiClient
    {
        internal ControlClient() : base(ConfigurationManager.AppSettings["ApiHost"]) { }

        public async Task<object> GetAllBlockStates()
        {
            await Task.FromResult(0);
            throw new NotImplementedException();
        }

        public async Task<BlockStateModel> GetBlockState(int blockId)
        {
            await Task.FromResult(0);
            throw new NotImplementedException();
        }

        public async Task<ActionInstanceModel> GetAllActionInstances()
        {
            await Task.FromResult(0);
            throw new NotImplementedException();
        }
    }
}
