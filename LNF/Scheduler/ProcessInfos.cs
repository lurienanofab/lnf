using System.Collections.Generic;

namespace LNF.Scheduler
{
    public static class ProcessInfos
    {
        public static List<ReservationProcessInfoItem> CreateReservationProcessInfoItems(IEnumerable<IReservationProcessInfo> items)
        {
            var result = new List<ReservationProcessInfoItem>();
            foreach (var i in items)
            {
                result.Add(CreateReservationProcessInfoItem(i));
            }
            return result;
        }

        public static ReservationProcessInfoItem CreateReservationProcessInfoItem(IReservationProcessInfo item)
        {
            return new ReservationProcessInfoItem
            {
                ReservationProcessInfoID = item.ReservationProcessInfoID,
                ProcessInfoID = item.ProcessInfoID,
                ReservationID = item.ReservationID,
                ProcessInfoLineID = item.ProcessInfoLineID,
                Value = item.Value,
                Special = item.Special,
                RunNumber = item.RunNumber,
                ChargeMultiplier = item.ChargeMultiplier,
                Active = item.Active
            };
        }
    }
}
