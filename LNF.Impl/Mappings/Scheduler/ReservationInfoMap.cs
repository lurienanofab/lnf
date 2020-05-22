using LNF.Impl.Repository.Scheduler;

namespace LNF.Impl.Mappings.Scheduler
{
    internal class ReservationInfoMap : ReservationInfoBaseMap<ReservationInfo>
    {
        internal ReservationInfoMap()
        {
            Table("v_ReservationInfo");
            Id(x => x.ReservationID);
        }
    }
}