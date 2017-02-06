using System.Threading.Tasks;

namespace OnlineServices.Api.Scheduler
{
    public class SchedulerServiceClient : ApiClient
    {
        internal SchedulerServiceClient(ApiClientOptions options) : base(options) { }

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
    }
}
