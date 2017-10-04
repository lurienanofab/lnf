using LNF.Models.Data;
using LNF.Models.Scheduler;
using System.Configuration;
using System.Threading.Tasks;

namespace OnlineServices.Api.Scheduler
{
    public class SchedulerServiceClient : ApiClient
    {
        public SchedulerServiceClient() : base(ConfigurationManager.AppSettings["ApiHost"]) { }

        public async Task<bool> RunFiveMinuteTask()
        {
            return await Get<bool>("scheduler/service/task-5min");
        }

        public async Task<bool> RunDailyTask()
        {
            return await Get<bool>("scheduler/service/task-daily");
        }

        public async Task<bool> RunMonthlyTask()
        {
            return await Get<bool>("scheduler/service/task-monthly");
        }

        public async Task<DataFeedModel<ExpiringCard>> GetExpiringCards()
        {
            return await Get<DataFeedModel<ExpiringCard>>("scheduler/service/expiring-cards");
        }

        public async Task<int> SendExpiringCardsEmail()
        {
            return await Get<int>("scheduler/service/expiring-cards/email");
        }
    }
}
