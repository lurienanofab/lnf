using LNF.Impl.Repository.Scheduler;

namespace LNF.Impl.Mappings.Scheduler
{
    internal class ReservationInviteeInfoMap : ReservationInfoBaseMap<ReservationInviteeInfo>
    {
        internal ReservationInviteeInfoMap()
        {
            Table("v_ReservationInviteeInfo");
            CompositeId()
                .KeyProperty(x => x.ReservationID)
                .KeyProperty(x => x.InviteeID);
            Map(x => x.InviteeLName);
            Map(x => x.InviteeFName);
            Map(x => x.InviteePrivs);
            Map(x => x.InviteeActive);
        }
    }
}
