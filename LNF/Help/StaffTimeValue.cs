using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LNF.Help
{
    public class StaffTimeValue
    {
        public StaffTimeValue()
        {
            Value = null;
        }

        public StaffTimeValue(TimeSpan? ts)
        {
            Value = ts;
        }

        public StaffTimeValue(TimeSpan ts)
        {
            Value = ts;
        }

        public StaffTimeValue(string ts)
        {
            Value = StaffTimeValue.Parse(ts);
        }

        public TimeSpan? Value;

        public override string ToString()
        {
            if (Value == null)
                return string.Empty;

            TimeSpan ts = (TimeSpan)Value;
            DateTime temp = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, ts.Hours, ts.Minutes, ts.Seconds);

            return temp.ToString("hh:mm tt").ToLower();
        }

        public static TimeSpan? Parse(string time)
        {
            if (!string.IsNullOrEmpty(time))
            {
                MatchCollection matches = Regex.Matches(time, "^([0-9]{2}):([0-9]{2}) ([ap]m)$");
                if (matches.Count == 1)
                {
                    Match m = matches[0];
                    if (m.Groups.Count == 4)
                    {
                        int hh = Convert.ToInt32(m.Groups[1].Value);
                        int mm = Convert.ToInt32(m.Groups[2].Value);
                        string ampm = m.Groups[3].Value;
                        hh = hh + ((ampm == "pm" && hh < 12) ? 12 : 0);
                        return new TimeSpan(hh, mm, 0) as TimeSpan?;
                    }
                }
            }
            return null;
        }
    }
}
