using LNF.Control;
using LNF.Repository.Control;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace LNF.Impl.Control.Wago
{
    public class WagoControl : IControlProvider
    {
        private HttpClient _client;

        public Uri Host { get; set; }

        private HttpClient GetClient()
        {
            if (_client == null)
            {
                _client = new HttpClient();
                _client.BaseAddress = Host;
            }

            return _client;
        }

        public async Task<PointResponse> Cancel(Point point)
        {
            var msg = await GetClient().GetAsync(string.Format("wago/point/{0}/cancel", point.PointID));
            var result = await msg.Content.ReadAsAsync<PointResponse>();
            return result;
        }

        public async Task<BlockResponse> GetBlockState(Block block)
        {
            var hc = GetClient();

            string requestUri = string.Format("wago/block/{0}", block.BlockID);

            var msg = await hc.GetAsync(requestUri);

            if (msg.IsSuccessStatusCode)
            {
                var result = await msg.Content.ReadAsAsync<BlockResponse>();
                return result;
            }
            else
            {
                string err = await msg.Content.ReadAsStringAsync();
                throw new InvalidOperationException(string.Format("[{0}:{1} {2}{3}]{4}{5}", (int)msg.StatusCode, msg.StatusCode, hc.BaseAddress, requestUri, Environment.NewLine, err));
            }
        }

        public async Task<PointResponse> SetPointState(Point point, bool state, uint duration)
        {
            var msg = await GetClient().GetAsync(string.Format("wago/point/{0}?state={1}&duration={2}", point.PointID, state, duration));
            var result = await msg.Content.ReadAsAsync<PointResponse>();
            return result;
        }

        public void Dispose()
        {
            if (_client != null)
                _client.Dispose();
        }
    }
}
