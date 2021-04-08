using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Scheduler;

namespace LNF.Impl.Mappings.Scheduler
{
    internal class ReservationInviteeItemMap : ClassMap<ReservationInviteeItem>
    {
        internal ReservationInviteeItemMap()
        {
            Schema("sselScheduler.dbo");
            Table("v_ReservationInviteeItem");
            ReadOnly();
            CompositeId()
                .KeyProperty(x => x.InviteeID)
                .KeyProperty(x => x.ReservationID);
            Map(x => x.ResourceID);
            Map(x => x.ProcessTechID);
            Map(x => x.BeginDateTime);
            Map(x => x.EndDateTime);
            Map(x => x.ActualBeginDateTime);
            Map(x => x.ActualEndDateTime);
            Map(x => x.IsStarted);
            Map(x => x.IsActive);
            Map(x => x.InviteeActive);
            Map(x => x.InviteeLName);
            Map(x => x.InviteeFName);
            Map(x => x.InviteePrivs);
        }
    }
}
