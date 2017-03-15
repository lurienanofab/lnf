using LNF.Cache;
using LNF.Data;
using LNF.Logging;
using LNF.Repository;
using LNF.Repository.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
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
            return Utility.TableDump(dt, false);
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
                    sb.AppendLine(@"<table style=""border-collapse: collapse;"">");
                    sb.AppendLine("<thead>");
                    sb.AppendLine("<tr>");
                    foreach (DataColumn dc in columns)
                    {
                        sb.AppendLine(@"<th style=""font-size: 9pt; padding: 5px; background-color: #D2D2D2; border: solid 1px #808080;"">" + dc.ColumnName + "</th>");
                    }
                    if (showRowState)
                        sb.Append(@"<th style=""font-size: 9pt; padding: 5px; background-color: #D2D2D2; border: solid 1px #808080;"">RowState</th>");
                    sb.AppendLine("</tr>");
                    sb.AppendLine("</thead>");
                    sb.AppendLine("<tbody>");
                    foreach (DataRow dr in rows)
                    {
                        sb.AppendLine("<tr>");
                        if (dr.RowState == DataRowState.Deleted)
                        {
                            sb.AppendLine(@"<td colspan=""" + columns.Count.ToString() + @""" style=""font-size: 9pt; padding: 5px; border: solid 1px #808080; font-style: italic"">deleted</td>");
                        }
                        else
                        {
                            foreach (DataColumn dc in columns)
                            {
                                sb.AppendLine(@"<td style=""font-size: 9pt; padding: 5px; border: solid 1px #808080;"">" + dr[dc.ColumnName].ToString() + "</td>");
                            }
                        }
                        if (showRowState)
                            sb.Append(@"<td style=""font-size: 9pt; padding: 5px; border: solid 1px #808080;"">" + dr.RowState.ToString() + "</th>");
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

        [Obsolete("Replaced by LNF.Repository.RepositoryUtility.ConvertTo")]
        public static T ConvertTo<T>(object obj, T defval)
        {
            if (obj == null)
                return defval;

            if (obj == DBNull.Value)
                return defval;

            T result = default(T);

            try
            {
                result = (T)Convert.ChangeType(obj, RepositoryUtility.GetUnderlyingType(typeof(T)));
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

        [Obsolete("Replaced by LNF.Repository.RepositoryUtility.TryConvertTo")]
        public static bool TryConvertTo<T>(object value, out T result, T defval)
        {
            result = Utility.ConvertTo(value, defval); ;
            return !result.Equals(defval);
        }

        //I want to use Insert commands (because this is truly what's happening)
        //The problem is that the rowstate for dt is unchanged, thus nothing gets pushed to the DB
        //Doing a dt.copy has the same problem
        public static DataTable CopyDT(DataTable dtIn)
        {
            DataRow ndr;
            DataTable dtOut = new DataTable();
            dtOut = dtIn.Clone();
            foreach (DataRow dr in dtIn.Rows)
            {
                ndr = dtOut.NewRow();
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
            string result = Providers.Serialization.Json.Serialize(obj);
            return result;
        }

        /// <summary>
        /// Finds the next business day after the first day of the month.
        /// </summary>
        public static DateTime NextBusinessDay(DateTime d)
        {
            return NextBusinessDay(d, CacheManager.Current.GetBusinessDay());
        }

        public static DateTime NextBusinessDay(DateTime d, int count)
        {
            DateTime result = d;
            bool isHoliday;
            int dayNum = 0;

            var holidays = DA.Current.Query<Holiday>().Where(x => x.HolidayDate >= result && x.HolidayDate < result.AddMonths(1)).ToList();

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

        public static bool IsBeforeNextBusinessDay(DateTime d)
        {
            var nbd = NextBusinessDay(d.FirstOfMonth());

            if (d.Date < nbd)
                return true;
            else
                return false;
        }

        public static bool IsFirstBusinessDay(DateTime d)
        {
            if (d.Date == NextBusinessDay(d))
                return true;
            else
                return false;
        }

        public static bool IsKiosk()
        {
            return LNF.Scheduler.KioskUtility.IsKiosk();
        }

        public static bool IsMobile()
        {
            if (PreferredMobileView() == "standard")
                return false;
            else
                return Utility.IsTablet() | Utility.IsPhone();
        }

        public static bool IsTablet()
        {
            bool result = false;

            if (PreferredMobileView() == "tablet")
                return true;

            if (!string.IsNullOrEmpty(Providers.Context.Current.GetRequestUserAgent()) && Providers.Context.Current.GetRequestUserAgent().ToLower().Contains("ipad"))
                return true;

            if (!string.IsNullOrEmpty(Providers.Context.Current.GetRequestUserAgent()) && Providers.Context.Current.GetRequestUserAgent().ToLower().Contains("android") && !Providers.Context.Current.GetRequestUserAgent().ToLower().Contains("mobile"))
                return true;

            return result;
        }

        public static bool IsPhone()
        {
            bool result = false;

            if (PreferredMobileView() == "phone")
                return true;

            if (Providers.Context.Current.GetRequestUserAgent().ToLower().Contains("iphone"))
                return true;

            if (Providers.Context.Current.GetRequestUserAgent().ToLower().Contains("android") && Providers.Context.Current.GetRequestUserAgent().ToLower().Contains("mobile"))
                return true;

            return result;
        }

        public static string PreferredMobileView()
        {
            string result = string.Empty;

            if (Providers.Context.Current.GetRequestCookieValue("lnf_mobile_pref_view") != null)
            {
                result = Providers.Context.Current.GetRequestCookieValue("lnf_mobile_pref_view");
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
            return Utility.YearData(Count, EndYear);
        }

        public static DataTable YearData(int Count, int EndYear)
        {
            int StartYear = EndYear - Count + 1;
            DateTime StartDate = new DateTime(StartYear, 1, 1);
            DateTime EndDate = new DateTime(EndYear, 1, 1);
            return Utility.YearData(StartDate, EndDate);
        }

        public static DataTable YearData(DateTime StartDate)
        {
            return Utility.YearData(StartDate, DateTime.Now);
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
            return RepositoryUtility.ConvertTo<TKey>(source.GetType().GetProperty(propName).GetValue(source, null), default(TKey));
        }

        public static string Left(string s, int length)
        {
            if (string.IsNullOrEmpty(s))
                return s;

            if (s.Length > length)
                return s.Substring(0, length);
            else
                return s;
        }

        public static object DBNullCheck(object value, bool test)
        {
            if (test) return DBNull.Value;
            else return value;
        }

        public static object ConvertNullableIntToObject(int? i)
        {
            if (i == null)
                return DBNull.Value;
            else
                return i.Value;
        }

        public static object ConvertNullableDateTimeToObject(DateTime? d)
        {
            if (d == null)
                return DBNull.Value;
            else
                return d.Value;
        }

        public static DateTime? ConvertObjectToNullableDateTime(object obj)
        {
            if (obj == DBNull.Value)
                return null;
            else
            {
                try
                {
                    return Convert.ToDateTime(obj);
                }
                catch
                {
                    return null;
                }
            }
        }

        public static int? ConvertObjectToNullableInt(object obj)
        {
            if (obj == DBNull.Value)
                return null;
            else
            {
                try
                {
                    return Convert.ToInt32(obj);
                }
                catch
                {
                    return null;
                }
            }
        }

        public static decimal? ConvertObjectToNullableDecimal(object obj)
        {
            if (obj == DBNull.Value)
                return null;
            else
            {
                try
                {
                    return Convert.ToDecimal(obj);
                }
                catch
                {
                    return null;
                }
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
                result = order.Split(',').Select(x => RepositoryUtility.ConvertTo(x, 0)).ToArray();
            return result;
        }

        [Obsolete("Replaced by LNF.Repository.RepositoryUtility.IsOverlapped")]
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

        [Obsolete("Use LNF.Logging.Logger")]
        public static void LogToFile(string message, params object[] args)
        {
            Logger.Write(TextLogMessage.Create("default", message, args));
        }

        public static T ParseEnum<T>(string value) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("T must be an enum");

            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static string EnumName(Enum value)
        {
            return Enum.GetName(value.GetType(), value);
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

        /// <summary>
        /// Gets a url for static files (css, js, images, etc.) in the shared static file source
        /// </summary>
        public static string GetStaticUrl(string path)
        {
            //example: <add key="StaticHost" value="//ssel-apps.eecs.umich.edu/static/"/>

            string defaultHost = "//ssel-apps.eecs.umich.edu/static/";
            string host = ConfigurationManager.AppSettings["StaticHost"];
            if (string.IsNullOrEmpty(host))
                host = defaultHost;
            return host + path;
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
    }
}
