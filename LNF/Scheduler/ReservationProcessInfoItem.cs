using LNF.Repository.Scheduler;
using System;

namespace LNF.Scheduler
{
    [Obsolete("Use LNF.Models.Scheduler.ReservationProcessInfoItem and LNF.Scheduler.IProcessInfoManager")]
    public class ReservationProcessInfoItem
    {
        public int ReservationProcessInfoID { get; set; }
        public int ReservationID { get; set; }
        public int ProcessInfoLineID { get; set; }
        public int ProcessInfoID { get; set; }
        public double Value { get; set; }
        public bool Special { get; set; }
        public int RunNumber { get; set; }
        public double ChargeMultiplier { get; set; }
        public bool Active { get; set; }

        public static ReservationProcessInfoItem Create(ReservationProcessInfo source)
        {
            //var pil = CacheManager.Current.ProcessInfoLines(source.ProcessInfoLine.ProcessInfoID).First(x => x.ProcessInfoLineID == source.ProcessInfoLine.ProcessInfoLineID);

            var result = new ReservationProcessInfoItem()
            {
                ReservationProcessInfoID = source.ReservationProcessInfoID,
                ReservationID = source.Reservation.ReservationID,
                ProcessInfoLineID = source.ProcessInfoLine.ProcessInfoLineID,
                ProcessInfoID = source.ProcessInfoLine.ProcessInfoID,
                Value = source.Value,
                Special = source.Special,
                RunNumber = source.RunNumber,
                ChargeMultiplier = source.ChargeMultiplier,
                Active = source.Active
            };

            return result;
        }
    }
}
