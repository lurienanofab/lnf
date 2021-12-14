using System;

namespace LNF.Scheduler
{
    public class AvailableReservationMinutesResult
    {
        /// <summary>
        /// Overbooked, currently reserved minutes exceeds max reservable (MaxAlloc).
        /// </summary>
        public static string ReasonA => "A";

        /// <summary>
        /// The user has more minutes available based on MaxAlloc than there are before the next reservation starts, so the available minutes is TimeUntilNext.
        /// </summary>
        public static string ReasonB => "B";

        /// <summary>
        /// There are enough minutes based on MaxAlloc to reserve all the way up to MaxReserveTime, so the user is not limited to less than MaxReservTime and the MaxReservTime limit is enforced by the application (AvailableReservationMinutes = ReservableMinutes in this case however this gets ignored).
        /// </summary>
        public static string ReasonC => "C";

        /// <summary>
        /// There are fewer minutes available than MaxReservTime so available is limited by ReservableMinutes instead of MaxReservTime (multiplied by -1 as a flag).
        /// </summary>
        public static string ReasonD => "D";

        public int AvailableReservationMinutes { get; set; }
        public int ReservableMinutes { get; set; }
        public int ReservedMinutes { get; set; }
        public int TimeUntilNext { get; set; }
        public string Reason { get; set; }

        public TimeSpan GetMaxDuration() => TimeSpan.FromMinutes(AvailableReservationMinutes);
    }
}
