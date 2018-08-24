using LNF.Control;
using LNF.Repository.Control;
using RestSharp;
using System;
using System.Threading.Tasks;

namespace LNF.Impl.Control.Wago
{
    public class WagoControlService : IControlService
    {
        private RestClient _client;

        public Uri Host { get; }

        public WagoControlService()
        {
            var controlElement = ServiceProvider.GetConfigurationSection().Control;
            if (controlElement.ElementInformation.IsPresent)
                Host = controlElement.Host;
        }

        private RestClient GetClient()
        {
            if (_client == null)
            {
                if (Host == null)
                    throw new Exception("The control configuration element is missing.");

                _client = new RestClient(Host);
            }

            return _client;
        }

        public Task<BlockResponse> GetBlockState(Block block)
        {
            var request = new RestRequest("wago/block/{blockId}");
            request.AddUrlSegment("blockId", block.BlockID);
            return GetSuccessfulResult<BlockResponse>(request);            
        }

        public Task<PointResponse> SetPointState(Point point, bool state, uint duration)
        {
            var request = new RestRequest("wago/block/{blockId}/point/{pointId}");
            request.AddUrlSegment("blockId", point.Block.BlockID);
            request.AddUrlSegment("pointId", point.PointID);
            request.AddParameter("state", state);
            request.AddParameter("duration", duration);
            return GetSuccessfulResult<PointResponse>(request);
        }

        public Task<PointResponse> Cancel(Point point)
        {
            var request = new RestRequest("wago/block/{blockId}/point/{pointId}/cancel");
            request.AddUrlSegment("blockId", point.Block.BlockID);
            request.AddUrlSegment("pointId", point.PointID);
            return GetSuccessfulResult<PointResponse>(request);
        }

        private async Task<T> GetSuccessfulResult<T>(IRestRequest request)
        {
            var client = GetClient();
            var response = await client.ExecuteGetTaskAsync<T>(request);

            if (response.IsSuccessful)
                return response.Data;
            else
                throw new InvalidOperationException($"[{(int)response.StatusCode}:{response.StatusCode} {client.BaseUrl}{request.Resource}]{Environment.NewLine}{response.ErrorMessage}");
        }

        public void Dispose()
        {
            _client = null;
        }
    }
}
