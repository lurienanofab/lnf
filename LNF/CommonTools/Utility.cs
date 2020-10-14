using LNF.Cache;
using LNF.Data;
using LNF.Scheduler;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace LNF.CommonTools
{
    public static class Utility
    {
        public static string Version()
        {
            return "79";
        }

        public static string TableDump(DataTable dt)
        {
            return TableDump(dt, false);
        }

        public static string TableDump(DataTable dt, bool showRowState)
        {
            return TableDump(dt, showRowState, "html");
        }

        public static string TableDump(DataTable dt, string format)
        {
            return TableDump(dt, false, format);
        }

        public static string TableDump(DataTable dt, bool showRowState, string format)
        {
            return TableDump(dt.AsEnumerable(), dt.Columns, showRowState, format);
        }

        public static string TableDump(IEnumerable<DataRow> rows, bool showRowState = false)
        {
            if (rows == null || rows.Count() == 0)
                return "There are no rows to display.";
            else
                return TableDump(rows, rows.First().Table.Columns, showRowState, "html");
        }

        public static string TableDump(IEnumerable<DataRow> rows, DataColumnCollection columns, bool showRowState, string format)
        {
            StringBuilder sb = new StringBuilder();
            switch (format)
            {
                case "html":
                case "html-bootstrap":
                    var dict = new Dictionary<string, IDictionary<string, string>>()
                    {
                        { "html", new Dictionary<string, string>() {
                            { "table-style", "border-collapse: collapse;" },
                            { "table-class", "" },
                            { "th-style", "font-size: 9pt; padding: 5px; background-color: #D2D2D2; border: solid 1px #808080;" },
                            { "th-class", "" },
                            { "td-deleted-style", "font-size: 9pt; padding: 5px; border: solid 1px #808080; font-style: italic;" },
                            { "td-deleted-class", "" },
                            { "td-style", "font-size: 9pt; padding: 5px; border: solid 1px #808080;" },
                            { "td-class", "" }
                        }},
                        { "html-bootstrap", new Dictionary<string, string>() {
                            { "table-style", "" },
                            { "table-class", "table table-striped" },
                            { "th-style", "" },
                            { "th-class", "" },
                            { "td-deleted-style", "font-style: italic;" },
                            { "td-deleted-class", "text-muted" },
                            { "td-style", "" },
                            { "td-class", "" }
                        }}
                    };

                    sb.AppendLine(string.Format(@"<table class=""{0}"" style=""{1}"">", dict[format]["table-class"], dict[format]["table-style"]));
                    sb.AppendLine("<thead>");
                    sb.AppendLine("<tr>");
                    foreach (DataColumn dc in columns)
                    {
                        sb.AppendLine(string.Format(@"<th class=""{0}"" style=""{1}"">{2}</th>", dict[format]["th-class"], dict[format]["th-style"], dc.ColumnName));
                    }
                    if (showRowState)
                        sb.Append(string.Format(@"<th class=""{0}"" style=""{1}"">RowState</th>", dict[format]["th-class"], dict[format]["th-style"]));
                    sb.AppendLine("</tr>");
                    sb.AppendLine("</thead>");
                    sb.AppendLine("<tbody>");
                    foreach (DataRow dr in rows)
                    {
                        sb.AppendLine("<tr>");
                        if (dr.RowState == DataRowState.Deleted)
                        {
                            sb.AppendLine(string.Format(@"<td colspan=""{0}"" class=""{1}"" style=""{2}"">deleted</td>", columns.Count, dict[format]["td-deleted-class"], dict[format]["td-deleted-style"]));
                        }
                        else
                        {
                            foreach (DataColumn dc in columns)
                            {
                                sb.AppendLine(string.Format(@"<td class=""{0}"" style=""{1}"">{2}</td>", dict[format]["td-class"], dict[format]["td-style"], dr[dc.ColumnName]));
                            }
                        }
                        if (showRowState)
                            sb.Append(string.Format(@"<td class=""{0}"" style=""{1}"">{2}</td>", dict[format]["td-class"], dict[format]["td-style"], dr.RowState));
                        sb.AppendLine("</tr>");
                    }
                    sb.AppendLine("</tbody>");
                    sb.AppendLine("</table>");
                    return sb.ToString();
                case "csv":
                    List<string> cols = new List<string>();
                    List<string> items = null;

                    foreach (DataColumn dc in columns)
                        cols.Add(dc.ColumnName);

                    //header
                    sb.AppendLine(string.Join(",", cols));

                    //items
                    foreach (DataRow dr in rows)
                    {
                        items = new List<string>();
                        foreach (string c in columns)
                            items.Add(dr[c].ToString());
                        sb.AppendLine(string.Join(",", items));
                    }

                    return sb.ToString();
                default:
                    throw new Exception("Invalid format.");
            }

        }

        public static string RowDump(DataRow dr)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(@"<table style=""border-collapse: collapse; font-size: 9pt;"">");
            sb.AppendLine("<thead>");
            sb.AppendLine("<tr>");
            foreach (DataColumn dc in dr.Table.Columns)
            {
                sb.AppendLine(string.Format(@"<th style=""padding: 5px; background-color: #D2D2D2; border: solid 1px #808080;"">{0}</th>", dc.ColumnName));
            }
            sb.AppendLine(@"<th style=""padding: 5px; background-color: #D2D2D2; border: solid 1px #808080;"">RowState</th>");
            sb.AppendLine("</tr>");
            sb.AppendLine("</thead>");
            sb.AppendLine("<tbody>");
            sb.AppendLine("<tr>");
            foreach (DataColumn dc in dr.Table.Columns)
            {
                sb.AppendLine(string.Format(@"<td style=""padding: 5px; border: solid 1px #808080;"">{0}</td>", dr[dc.ColumnName]));
            }
            sb.AppendLine(string.Format(@"<td style=""padding: 5px; border: solid 1px #808080;"">{0}</td>", dr.RowState));
            sb.AppendLine("</tr>");
            sb.AppendLine("</tbody>");
            sb.AppendLine("</table>");

            return sb.ToString();
        }

        public static double Round(double value, int digits)
        {
            return Math.Round(value, digits, MidpointRounding.AwayFromZero);
        }

        public static T ConvertTo<T>(object obj, T defval)
        {
            if (obj == null)
                return defval;

            if (obj == DBNull.Value)
                return defval;

            T result = default(T);

            try
            {
                result = (T)Convert.ChangeType(obj, GetUnderlyingType(typeof(T)));
            }
            catch
            {
                try
                {
                    result = (T)obj;
                }
                catch
                {
                    result = defval;
                }
            }

            return result;
        }

        /// <summary>
        /// Tries to convert an object to the specified type
        /// </summary>
        /// <typeparam name="T">The expected return value</typeparam>
        /// <param name="value">The object which will be set to the converted value</param>
        /// <param name="result">A value that indicates if the conversion was successful</param>
        /// <param name="defval">The value to set result to if conversion fails for any reason</param>
        /// <returns>True if the conversion was successful, otherwise false</returns>
        public static bool TryConvertTo<T>(object value, out T result, T defval)
        {
            result = ConvertTo(value, defval); ;
            return !result.Equals(defval);
        }

        public static int ConvertToInt32(object v)
        {
            return IsDBNullOrNull(v)
                ? 0
                : Convert.ToInt32(v);
        }

        public static double ConvertToDouble(object v)
        {
            return IsDBNullOrNull(v)
                ? 0
                : Convert.ToDouble(v);
        }

        /// <summary>
        /// Returns the underlying type of the given type. If the given type is nullable the underlying type is returned. Otherwise the given type is returned
        /// </summary>
        /// <param name="t">The specified type</param>
        /// <returns>A type that is the underlying type of the specified type</returns>
        public static Type GetUnderlyingType(Type t)
        {
            Type result;
            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
                result = Nullable.GetUnderlyingType(t);
            else
                result = t;
            return result;
        }

        /// <summary>
        /// Returns null if value is zero, otherwise returns value.
        /// </summary>
        public static int? NullIfZero(int value)
        {
            if (value == 0)
                return null;
            else
                return value;
        }

        /// <summary>
        /// Creates a copy by adding rows to a new table so that DataRowState will be Added instead of Unchanged.
        /// </summary>
        public static DataTable CopyDataTable(DataTable dtIn)
        {
            DataTable dtOut = dtIn.Clone();
            foreach (DataRow dr in dtIn.Rows)
            {
                DataRow ndr = dtOut.NewRow();
                ndr.ItemArray = dr.ItemArray;
                dtOut.Rows.Add(ndr);
            }
            return dtOut;
        }

        public static bool StringToBoolean(object obj, bool defval = false)
        {
            if (obj == null)
                return defval;

            string str = obj.ToString();

            if (string.IsNullOrEmpty(str))
                return defval;

            switch (str.Trim().ToLower())
            {
                case "true":
                case "yes":
                case "1":
                case "on":
                    return true;
                case "false":
                case "no":
                case "0":
                    return false;
                default:
                    return defval;
            }
        }

        public static bool CompareFlags(object flag1, object flag2)
        {
            return ((int)flag1 & (int)flag2) > 0;
        }

        public static string ToJson(object obj)
        {
            string result = ServiceProvider.Current.Utility.Serialization.Json.Serialize(obj);
            return result;
        }

        public static bool InCurrentPeriod(IReservation rsv)
        {
            DateTime d = ((rsv.ActualEndDateTime == null) ? rsv.EndDateTime : rsv.ActualEndDateTime.Value).Date;
            DateTime period = new DateTime(d.Year, d.Month, 1);
            return Utility.IsCurrentPeriod(period);
        }

        /// <summary>
        /// Finds the next business day after the first day of the month.
        /// </summary>
        public static DateTime NextBusinessDay(DateTime d, IEnumerable<IHoliday> holidays)
        {
            return NextBusinessDay(d, CacheManager.Current.GetBusinessDay(), holidays);
        }

        /// <summary>
        /// Finds the next business day after the first day of the month.
        /// </summary>
        public static DateTime NextBusinessDay(DateTime d, int count, IEnumerable<IHoliday> holidays)
        {
            DateTime result = d;
            bool isHoliday;
            int dayNum = 0;

            // this method gets used in a loop so pass in the holidays to avoid executing this query over and over
            //var holidays = DA.Current.Query<Holiday>().Where(x => x.HolidayDate >= d && x.HolidayDate < d.AddMonths(1)).ToList();

            while (dayNum < count)
            {
                isHoliday = holidays.Any(x => x.HolidayDate == result)
                    || result.DayOfWeek == DayOfWeek.Saturday
                    || result.DayOfWeek == DayOfWeek.Sunday;

                if (!isHoliday) dayNum += 1;

                result = result.AddDays(1);
            }

            return result;
        }

        public static bool IsBeforeNextBusinessDay(DateTime d, IEnumerable<IHoliday> holidays)
        {
            var nbd = NextBusinessDay(d.FirstOfMonth(), holidays);

            if (d.Date < nbd)
                return true;
            else
                return false;
        }

        public static bool IsFirstBusinessDay(DateTime d, IEnumerable<IHoliday> holidays)
        {
            if (d.Date == NextBusinessDay(d, holidays))
                return true;
            else
                return false;
        }

        public static IEnumerable<IHoliday> GetHolidays(DateTime sd, DateTime ed) => ServiceProvider.Current.Data.Holiday.GetHolidays(sd, ed);

        public static bool IsKiosk(IKioskRepository repo, string userHostAddress)
        {
            return Kiosks.Create(repo).IsKiosk(userHostAddress);
        }

        public static bool IsMobile(string userAgent, string preferredMobileView)
        {
            if (preferredMobileView == "standard")
                return false;
            else
                return IsTablet(userAgent, preferredMobileView) | IsPhone(userAgent, preferredMobileView);
        }

        public static bool IsTablet(string userAgent, string preferredMobileView)
        {
            bool result = false;

            if (preferredMobileView == "tablet")
                return true;

            if (!string.IsNullOrEmpty(userAgent) && userAgent.ToLower().Contains("ipad"))
                return true;

            if (!string.IsNullOrEmpty(userAgent) && userAgent.ToLower().Contains("android") && !userAgent.ToLower().Contains("mobile"))
                return true;

            return result;
        }

        public static bool IsPhone(string userAgent, string preferredMobileView)
        {
            bool result = false;

            if (preferredMobileView == "phone")
                return true;

            if (userAgent.ToLower().Contains("iphone"))
                return true;

            if (userAgent.ToLower().Contains("android") && userAgent.ToLower().Contains("mobile"))
                return true;

            return result;
        }

        public static string PreferredMobileView(Func<string, string> getCookieValue)
        {
            string result = string.Empty;

            if (getCookieValue("lnf_mobile_pref_view") != null)
            {
                result = getCookieValue("lnf_mobile_pref_view");
            }

            return result;
        }

        public static string GoogleAnalyticsTrackingID
        {
            get
            {
                string defval = "UA-23459384-2"; //this is the tracking id set up through the lnf.umich.edu Google domain
                string result = ConfigurationManager.AppSettings["GoogleAnalyticsTrackingID"];
                if (string.IsNullOrEmpty(result))
                    result = defval;
                return result;
            }
        }

        public static IEnumerable<DateTime> GetMonths()
        {
            for (int m = 1; m < 13; m++)
            {
                yield return new DateTime(DateTime.Now.Year, m, 1);
            }
        }

        public static IEnumerable<int> GetYears(int startYear)
        {
            return GetYears(startYear, DateTime.Now.Year);
        }

        public static IEnumerable<int> GetYears(int startYear, int endYear)
        {
            return Enumerable.Range(startYear, (endYear - startYear) + 1);
        }

        public static DataTable YearData(int Count)
        {
            int EndYear = DateTime.Now.Year;
            return YearData(Count, EndYear);
        }

        public static DataTable YearData(int Count, int EndYear)
        {
            int StartYear = EndYear - Count + 1;
            DateTime StartDate = new DateTime(StartYear, 1, 1);
            DateTime EndDate = new DateTime(EndYear, 1, 1);
            return YearData(StartDate, EndDate);
        }

        public static DataTable YearData(DateTime StartDate)
        {
            return YearData(StartDate, DateTime.Now);
        }

        public static DataTable YearData(DateTime StartDate, DateTime EndDate)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("YearValue", typeof(string));
            dt.Columns.Add("YearText", typeof(string));
            int StartYear = StartDate.Year;
            int EndYear = EndDate.Year;
            while (StartYear <= EndYear)
            {
                dt.Rows.Add(StartYear.ToString(), StartYear.ToString());
                StartYear++;
            }
            return dt;
        }

        public static DataRowView RowToView(DataRow dr)
        {
            DataTable dt = dr.Table;
            DataRowView drv = dt.DefaultView[dt.Rows.IndexOf(dr)];
            return drv;
        }

        public static bool TryGetValue(Hashtable hash, object key, out object result)
        {
            if (hash.ContainsKey(key))
            {
                result = hash[key];
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }

        public static T TryGetValue<T>(DataRow dr, string column, T defval)
        {
            if (dr.Table.Columns.Contains(column))
            {
                if (dr[column] != DBNull.Value)
                {
                    if (dr[column] != null)
                    {
                        try
                        {
                            T result = (T)dr[column];

                            if (result != null)
                                return result;
                        }
                        catch
                        {
                            return defval;
                        }
                    }
                }
            }

            return defval;
        }

        public static bool IsDBNullOrNull(object obj)
        {
            if (obj == null) return true;
            if (obj == DBNull.Value) return true;
            return false;
        }

        public static int NumberOfDaysInMonth(DateTime d)
        {
            DateTime fom = d.FirstOfMonth();
            DateTime nextMonth = fom.AddMonths(1);
            DateTime lastDayOfMonth = nextMonth.AddDays(-1);
            return lastDayOfMonth.Day;
        }

        public static string PropertyName<TSource, TKey>(Expression<Func<TSource, TKey>> exp)
        {
            string result = string.Empty;
            if (typeof(MemberExpression).IsAssignableFrom(exp.Body.GetType()))
                result = ((MemberExpression)exp.Body).Member.Name;
            return result;
        }

        public static TKey PropertyValue<TSource, TKey>(TSource source, Expression<Func<TSource, TKey>> exp)
        {
            string propName = PropertyName(exp);
            return ConvertTo<TKey>(source.GetType().GetProperty(propName).GetValue(source, null), default(TKey));
        }

        public static string Clip(string s, int length)
        {
            if (string.IsNullOrEmpty(s))
                return s;

            if (s.Length > length)
                return s.Substring(0, length);
            else
                return s;
        }

        public static object DBNullIf(object value, bool test)
        {
            if (test) return DBNull.Value;
            else return value;
        }

        public static object DBNullIf<T>(T? value) where T : struct
        {
            if (!value.HasValue)
                return DBNull.Value;
            else
                return value.Value;
        }

        public static DateTime? ConvertToNullableDateTime(object obj)
        {
            if (obj == DBNull.Value)
                return null;

            try
            {
                return Convert.ToDateTime(obj);
            }
            catch
            {
                return null;
            }
        }

        public static int? ConvertToNullableInt32(object obj)
        {
            if (obj == DBNull.Value)
                return null;

            try
            {
                return Convert.ToInt32(obj);
            }
            catch
            {
                return null;
            }
        }

        public static decimal? ConvertToNullableDouble(object obj)
        {
            if (obj == DBNull.Value)
                return null;

            try
            {
                return Convert.ToDecimal(obj);
            }
            catch
            {
                return null;
            }
        }

        public static TimeSpan? GetNullableTimeSpanFromMinutes(int? value)
        {
            if (value.HasValue)
                return TimeSpan.FromMinutes(value.Value);
            else
                return null;
        }

        public static int? GetNullableMinutesFromTimeSpan(TimeSpan? value)
        {
            if (value.HasValue)
                return Convert.ToInt32(value.Value.TotalMinutes);
            else
                return null;
        }

        public static int[] ConvertStringToIntArray(string s)
        {
            int[] result = null;
            if (string.IsNullOrEmpty(s))
                return result;
            string order = (s.StartsWith(",")) ? s.Substring(1) : s;
            if (!string.IsNullOrEmpty(order))
                result = order.Split(',').Select(x => ConvertTo(x, 0)).ToArray();
            return result;
        }

        public static bool Overlap(DateTime startRange1, DateTime? endRange1, DateTime startRange2, DateTime endRange2, bool exclusive = true)
        {
            //We always assume that the end date is EXCLUSIVE which means it
            //is not included in the range (note: this applies to BOTH ranges).
            //For example 5/1 to 6/1 does not include 6/1

            //Using <= and >= makes the ranges inclusive.
            //Do not mix: if <= is used then use >=, if < is used then use >
            //Othersise one range gets treated as inclusive and the other as exclusive.

            bool result;

            if (exclusive)
                result = startRange1 < endRange2 && (endRange1 == null || endRange1.Value > startRange2);
            else
                result = startRange1 <= endRange2 && (endRange1 == null || endRange1.Value >= startRange2);

            return result;
        }

        public static T ParseEnum<T>(string value) where T : struct, IConvertible
        {
            // this works for flag enums - e.g. value = "val1,val2,val4,..." returns "Val1, Val2, Val4, ..."
            if (!typeof(T).IsEnum)
                throw new ArgumentException("T must be an enum");

            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static string EnumName(Enum value)
        {
            return Enum.GetName(value.GetType(), value);
        }

        public static string EnumToString(Enum value, string separator = "|")
        {
            // The normal enum ToString() method returns "Value1, Value2, Value3, ..."
            // This method will return "Value1|Value2|Value3..." instead.
            return string.Join(separator, value.ToString().Split(',').Select(x => x.Trim()));
        }

        public static TResult GetValueOrDefault<TSource, TResult>(TSource source, Func<TSource, TResult> result, TResult defval = default(TResult))
        {
            if (source == null)
                return defval;
            else
                return result(source);
        }

        /// <summary>
        /// Creates a MD5 hash of the input.
        /// </summary>
        public static string Hash(string input)
        {
            var md5 = MD5.Create();
            var hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            var result = string.Concat(hashBytes.Select(b => b.ToString("x2")));
            return result;
        }

        public static T NewObject<T>()
        {
            T result;

            if (typeof(T).GetConstructor(Type.EmptyTypes) != null)
                result = Activator.CreateInstance<T>(); // type has a parameterless constructor so use Activator
            else
                result = default(T); // the best we can do

            return result;
        }

        /// <summary>
        /// Converts an object to a dictionary by relecting property names and values.
        /// </summary>
        public static IDictionary<string, object> ObjectToDictionary(object obj)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();

            if (obj != null)
            {
                result = obj.GetType().GetProperties().ToDictionary(
                    p => p.Name,
                    p => p.GetValue(obj, null)
                );
            }

            return result;
        }

        public static string GetQueryString(object obj)
        {
            if (obj == null) return string.Empty;
            return CreateQueryString(ObjectToDictionary(obj));
        }

        public static string CreateQueryString(IDictionary<string, object> dict)
        {
            if (dict == null || dict.Count == 0) return string.Empty;

            string result = string.Empty;

            string amp = "?";

            foreach (KeyValuePair<string, object> kvp in dict)
            {
                result += amp + kvp.Key + "=" + kvp.Value.ToString();
                amp = "&";
            }

            return result;
        }

        /// <summary>
        /// Indicates if the specified date is in the current period.
        /// </summary>
        public static bool IsCurrentPeriod(DateTime period)
        {
            DateTime sd = DateTime.Now.FirstOfMonth();
            DateTime ed = sd.AddMonths(1);
            return (period >= sd && period < ed);
        }

        public static string GetAppSetting(string key)
        {
            var result = ConfigurationManager.AppSettings[key];
            return result;
        }

        public static string GetRequiredAppSetting(string key)
        {
            var result = GetAppSetting(key);

            if (string.IsNullOrEmpty(result))
                throw new Exception($"Missing required appSetting: {key}");

            return result;
        }

        public static string GetGlobalSetting(string name)
        {
            var gs = ServiceProvider.Current.Data.GlobalSetting.GetGlobalSetting(name);
            if (gs == null) return null;
            return gs.SettingValue;
        }

        public static string GetRequiredGlobalSetting(string name)
        {
            var result = GetGlobalSetting(name);

            if (string.IsNullOrEmpty(result))
                throw new Exception($"Missing required GlobalSetting: {name}");

            return result;
        }

        public static IQueryable<T> ToQueryable<T>(T item)
        {
            var list = new List<T>();
            if (item != null) list.Add(item);
            var result = list.AsQueryable();
            return result;
        }

        public static DateTime Truncate(DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second, date.Kind);
        }

        public static DateTime? Truncate(DateTime? date)
        {
            if (date == null)
                return null;

            return Truncate(date.Value);
        }

        //https://stackoverflow.com/questions/857705/get-all-derived-types-of-a-type
        public static Type[] GetAssignableFromType<T>(IEnumerable<Assembly> assemblies)
        {
            var result = (from domainAssembly in assemblies
                          from assemblyType in domainAssembly.GetExportedTypes()
                          where typeof(T).IsAssignableFrom(assemblyType)
                          where assemblyType.IsSubclassOf(typeof(T)) && !assemblyType.IsAbstract
                          select assemblyType).ToArray();

            return result;
        }

        public static Type[] GetAssignableFromType(IEnumerable<Type> types, IEnumerable<Assembly> assemblies)
        {
            var result = new List<Type>();

            foreach (var t in types)
            {
                var assignable = (from domainAssembly in assemblies
                                  from assemblyType in domainAssembly.GetExportedTypes()
                                  where t.IsAssignableFrom(assemblyType)
                                  where assemblyType.IsSubclassOf(t) && !assemblyType.IsAbstract
                                  select assemblyType).ToArray();

                result.AddRange(assignable);
            }

            return result.ToArray();
        }
    }
}
