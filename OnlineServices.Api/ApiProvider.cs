using OnlineServices.Api.Billing;
using OnlineServices.Api.Control;
using OnlineServices.Api.Data;
using OnlineServices.Api.Inventory;
using OnlineServices.Api.Scheduler;
using System.Threading.Tasks;

namespace OnlineServices.Api
{
    public static class ApiProvider
    {
     
        public static async Task<ApiClient> NewGenericClient()
        {
            return new ApiClient(await GetOptions());
        }

        public static async Task<DataClient> NewDataClient()
        {
            return new DataClient(await GetOptions());
        }

        public static async Task<FeedClient> NewFeedClient()
        {
            return new FeedClient(await GetOptions());
        }

        public static async Task<ControlClient> NewControlClient()
        {
            return new ControlClient(await GetOptions());
        }

        public static async Task<BillingClient> NewBillingClient()
        {
            return new BillingClient(await GetOptions());
        }

        public static async Task<SchedulerClient> NewSchedulerClient()
        {
            return new SchedulerClient(await GetOptions());
        }

        public static async Task<SchedulerServiceClient> NewSchedulerServiceClient()
        {
            return new SchedulerServiceClient(await GetOptions());
        }

        public static async Task<InventoryClient> NewInventoryClient()
        {
            return new InventoryClient(await GetOptions());
        }

        public static async Task<ApiClientOptions> GetOptions()
        {
            return await AuthorizationManager.Current.GetOptionsForClientCredentials();
        }
    }
}
