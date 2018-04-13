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
        
        public Uri Host { get; set; }

        private RestClient GetClient()
        {
            if (_client == null)
            {
                _client = new RestClient(Host);
            }

            return _client;
        }

        public async Task<PointResponse> Cancel(Point point)
        {
            var request = new RestRequest("wago/point/{pointId}/cancel");
            request.AddUrlSegment("pointId", point.PointID);
            var response = await GetClient().ExecuteGetTaskAsync<PointResponse>(request);
            return response.Data;
        }

        public async Task<BlockResponse> GetBlockState(Block block)
        {
            var client = GetClient();

            var request = new RestRequest("wago/block/{blockId}");
            request.AddUrlSegment("blockId", block.BlockID);

            var response = await client.ExecuteGetTaskAsync<BlockResponse>(request);

            if (response.IsSuccessful)
                return response.Data;
            else
            {
                string err = response.ErrorMessage;
                throw new InvalidOperationException(string.Format("[{0}:{1} {2}{3}]{4}{5}", (int)response.StatusCode, response.StatusCode,  client.BaseUrl, request.Resource, Environment.NewLine, err));
            }
        }

        public async Task<PointResponse> SetPointState(Point point, bool state, uint duration)
        {
            var request = new RestRequest("wago/point/{pointId}");
            request.AddUrlSegment("pointId", point.PointID);
            request.AddParameter("state", state);
            request.AddParameter("duration", duration);

            var response = await GetClient().ExecuteGetTaskAsync<PointResponse>(request);

            return response.Data;
        }

        public void Dispose()
        {
            _client = null;
        }
    }
}
