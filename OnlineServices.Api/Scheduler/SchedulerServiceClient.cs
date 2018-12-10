using LNF.Models.Data;
using LNF.Models.Scheduler;
using LNF.Models.Scheduler.Service;

namespace OnlineServices.Api.Scheduler
{
    public class SchedulerServiceClient : ApiClient
    {
        public SchedulerServiceClient() : base(GetApiBaseUrl()) { }

        public FiveMinuteTaskResult RunFiveMinuteTask()
        {
            return Get<FiveMinuteTaskResult>("webapi/scheduler/service/task-5min");
        }

        public DailyTaskResult RunDailyTask()
        {
            return Get<DailyTaskResult>("webapi/scheduler/service/task-daily");
        }

        public MonthlyTaskResult RunMonthlyTask()
        {
            return Get<MonthlyTaskResult>("webapi/scheduler/service/task-monthly");
        }

        public DataFeedModel<ExpiringCard> GetExpiringCards()
        {
            return Get<DataFeedModel<ExpiringCard>>("webapi/scheduler/service/expiring-cards");
        }

        public int SendExpiringCardsEmail()
        {
            return Get<int>("webapi/scheduler/service/expiring-cards/email");
        }
    }
}
