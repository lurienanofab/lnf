using FluentNHibernate.Mapping;
using LNF.Models.Scheduler;
using LNF.Repository.Scheduler;

namespace LNF.Impl.Mappings.Scheduler
{
    internal class ActivityMap : ClassMap<Activity>
    {
        internal ActivityMap()
        {
            Schema("sselScheduler.dbo");
            Id(x => x.ActivityID);
            Map(x => x.ActivityName);
            Map(x => x.ListOrder);
            Map(x => x.Chargeable);
            Map(x => x.Editable);
            Map(x => x.AccountType).CustomType<ActivityAccountType>();
            Map(x => x.UserAuth);
            Map(x => x.InviteeType);
            Map(x => x.InviteeAuth);
            Map(x => x.StartEndAuth);
            Map(x => x.NoReservFenceAuth);
            Map(x => x.NoMaxSchedAuth);
            Map(x => x.Description);
            Map(x => x.IsActive);
            Map(x => x.IsFacilityDownTime);
        }
    }
}
