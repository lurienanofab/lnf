using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF.Help
{
    public class StaffTimeInfo
    {
        private DayOfWeek _DayOfWeek;
        public DayOfWeek DayOfWeek { get { return _DayOfWeek; } }
        public bool Checked { get; set; }
        public TimeRange AM { get; set; }
        public TimeRange PM { get; set; }

        public StaffTimeInfo(DayOfWeek dow)
        {
            _DayOfWeek = dow;
            Checked = false;
            AM = new TimeRange();
            PM = new TimeRange();
        }

        public string TimeRangeToString()
        {
            string result = string.Empty;
            if (AM.Start.Value != null)
            {
                TimeSpan ts = (TimeSpan)AM.Start.Value;
                result += FormatShortTime(ts);
            }
            if (AM.End.Value != null)
            {
                TimeSpan ts = (TimeSpan)AM.End.Value;
                if (!string.IsNullOrEmpty(result)) result += " - ";
                result += FormatShortTime(ts);
            }
            if (PM.Start.Value != null)
            {
                TimeSpan ts = (TimeSpan)PM.Start.Value;
                if (!string.IsNullOrEmpty(result)) result += ", ";
                result += FormatShortTime(ts);
            }
            if (PM.End.Value != null)
            {
                TimeSpan ts = (TimeSpan)PM.End.Value;
                if (!string.IsNullOrEmpty(result)) result += " - ";
                result += FormatShortTime(ts);
            }

            return result;
        }

        public string FormatShortTime(TimeSpan ts)
        {
            string result = string.Empty;
            int hh = (ts.Hours > 12) ? ts.Hours - 12 : ts.Hours;
            result += hh.ToString();
            if (ts.Minutes != 0) result += ":" + (ts.Minutes + 100).ToString().Substring(1);
            result += (ts.Hours < 12) ? "am" : "pm";
            return result;
        }

        public string DayOfWeekToShortString()
        {
            string result = string.Empty;
            switch (_DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    result = "Su";
                    break;
                case DayOfWeek.Monday:
                    result = "M";
                    break;
                case DayOfWeek.Tuesday:
                    result = "Tu";
                    break;
                case DayOfWeek.Wednesday:
                    result = "W";
                    break;
                case DayOfWeek.Thursday:
                    result = "Th";
                    break;
                case DayOfWeek.Friday:
                    result = "F";
                    break;
                case DayOfWeek.Saturday:
                    result = "Sa";
                    break;
            }
            return result;
        }
    }
}
