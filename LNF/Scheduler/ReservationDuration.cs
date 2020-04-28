using System;
using System.Configuration;
using System.Linq;

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

        public bool IsAfterHours()
        {
            string name = ConfigurationManager.AppSettings["AfterHoursName"];
            if (string.IsNullOrEmpty(name)) name = "A";

            var afterHours = ServiceProvider.Current.Reporting.AfterHours.GetAfterHours(name);

            int beginDayOfWeekIndex = (int)BeginDateTime.DayOfWeek + 1; //for DayOfWeekIndex Sunday = 1, but in c# DayOfWeek Sunday = 0

            var workHoursStart = afterHours.Where(x => x.DayOfWeekIndex == beginDayOfWeekIndex && !x.IsAfterHours).OrderBy(x => x.HourIndex).FirstOrDefault();

            if (workHoursStart == null)
            {
                // there are no work hours on this day (maybe it's Sunday)
                return true;
            }

            DateTime workStartTime = BeginDateTime.Date.AddHours(workHoursStart.HourIndex);

            if (BeginDateTime < workStartTime)
                return true;

            int endDayOfWeekIndex = (int)EndDateTime.DayOfWeek + 1;

            var workHoursEnd = afterHours.Where(x => x.DayOfWeekIndex == endDayOfWeekIndex && !x.IsAfterHours).OrderByDescending(x => x.HourIndex).FirstOrDefault();

            if (workHoursEnd == null)
                return true;

            // should be the first after-hour of the day, so we need to +1
            DateTime workEndTime = EndDateTime.Date.AddHours(workHoursEnd.HourIndex + 1);

            if (EndDateTime > workEndTime)
                return true;

            return false;
        }

        public override string ToString() => Duration.ToString();
    }
}
