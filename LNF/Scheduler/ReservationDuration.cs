using System;

namespace LNF.Scheduler
{
    public struct ReservationDuration
    {
        public readonly DateTime BeginDateTime;
        public readonly DateTime EndDateTime;
        public readonly TimeSpan Duration;

        public static ReservationDuration FromMinutes(DateTime beginDateTime, int minutes)
        {
            return new ReservationDuration(beginDateTime, TimeSpan.FromMinutes(minutes));
        }

        public static ReservationDuration FromMinutes(DateTime beginDateTime, double minutes)
        {
            return new ReservationDuration(beginDateTime, TimeSpan.FromMinutes(minutes));
        }

        public ReservationDuration(DateTime beginDateTime, TimeSpan duration)
        {
            BeginDateTime = beginDateTime;
            Duration = duration;
            EndDateTime = BeginDateTime.Add(duration);
        }
    }
}
