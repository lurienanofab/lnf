using FluentNHibernate.Mapping;
using LNF.Repository.Scheduler;

namespace LNF.Impl.Mappings.Scheduler
{
    public class ReservationInviteeInfoMap : ClassMap<ReservationInviteeInfo>
    {
        public ReservationInviteeInfoMap()
        {
            Schema("sselScheduler.dbo");
            Table("v_ReservationInviteeInfo");
            ReadOnly();
            CompositeId()
                .KeyProperty(x => x.ReservationID)
                .KeyProperty(x => x.InviteeID);
            Map(x => x.BeginDateTime);
            Map(x => x.EndDateTime);
            Map(x => x.ActualBeginDateTime);
            Map(x => x.ActualEndDateTime);
            Map(x => x.IsStarted);
            Map(x => x.IsActive);
            Map(x => x.ResourceID);
            Map(x => x.ResourceName);
            Map(x => x.LName);
            Map(x => x.FName);
            Map(x => x.Privs);
            Map(x => x.Active);
        }
    }
}
