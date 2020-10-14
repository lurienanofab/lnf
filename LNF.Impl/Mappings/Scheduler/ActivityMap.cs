using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Scheduler;
using LNF.Scheduler;

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
            Map(x => x.UserAuth).CustomType<ClientAuthLevel>();
            Map(x => x.InviteeType).CustomType<ActivityInviteeType>();
            Map(x => x.InviteeAuth).CustomType<ClientAuthLevel>();
            Map(x => x.StartEndAuth).CustomType<ClientAuthLevel>();
            Map(x => x.NoReservFenceAuth).CustomType<ClientAuthLevel>();
            Map(x => x.NoMaxSchedAuth).CustomType<ClientAuthLevel>();
            Map(x => x.Description);
            Map(x => x.IsActive);
            Map(x => x.IsFacilityDownTime);
        }
    }
}
