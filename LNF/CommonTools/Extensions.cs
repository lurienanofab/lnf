using LNF.CommonTools;
using LNF.DataAccess;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace LNF.CommonTools
{
    public static class Extensions
    {
        public static DateTime RoundUp(this DateTime dt, TimeSpan ts)
        {
            return new DateTime(((dt.Ticks + ts.Ticks - 1) / ts.Ticks) * ts.Ticks);
        }

        public static DateTime RoundDown(this DateTime dt, TimeSpan ts)
        {
            return new DateTime((dt.Ticks / ts.Ticks) * ts.Ticks);
        }

        public static IEnumerable<TResult> LeftJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
        {
            var join = new List<TResult>();

            foreach (var o in outer)
            {
                var matches = inner.Where(x => innerKeySelector(x).Equals(outerKeySelector(o))).DefaultIfEmpty();
                join.AddRange(matches.Select(m => resultSelector(o, m)));
            }

            return join;
        }

        public static IEnumerable<TResult> FullOuterJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
            where TInner : class
            where TOuter : class
        {
            var innerLookup = inner.ToLookup(innerKeySelector);
            var outerLookup = outer.ToLookup(outerKeySelector);

            var innerJoinItems = inner
                .Where(innerItem => !outerLookup.Contains(innerKeySelector(innerItem)))
                .Select(innerItem => resultSelector(null, innerItem));

            return outer
                .SelectMany(outerItem =>
                {
                    var innerItems = innerLookup[outerKeySelector(outerItem)];

                    return innerItems.Any() ? innerItems : new TInner[] { };
                }, resultSelector).Concat(innerJoinItems);
        }

        // the main purpose of this method is the null check
        public static TProperty ValueOf<TItem, TProperty>(this TItem item, Func<TItem, TProperty> fn) where TItem : IDataItem
        {
            if (item != null)
                return fn(item);
            else
                return default(TProperty);
        }

        public static string ToString(this Nullable<DateTime> date, string format = null)
        {
            if (date.HasValue)
            {
                if (!string.IsNullOrEmpty(format))
                    return date.Value.ToString(format);
                else
                    return date.Value.ToString();
            }

            return string.Empty;
        }

        ///<summary>
        /// Gets a value from a NamedValueCollection
        ///</summary>
        public static T Value<T>(this NameValueCollection nvc, string key, T defval)
        {
            return Utility.ConvertTo(nvc[key], defval);
        }

        ///<summary>
        /// Gets a boolean from a NamedValueCollection
        ///</summary>
        public static bool BoolValue(this NameValueCollection nvc, string key, bool defval)
        {
            return Utility.StringToBoolean(nvc[key], defval);
        }

        public static Dictionary<string, string> ToDictionary(this NameValueCollection nvc)
        {
            string[] keys = nvc.AllKeys;
            Dictionary<string, string> result = keys.ToDictionary(k => k, v => nvc[v]);
            return result;
        }

        ///<summary>
        /// Gets a value from a nullable object or returns DBNull.Value when the object is null.
        ///</summary>
        public static object GetValueOrDBNull<T>(this Nullable<T> nullable) where T : struct
        {
            if (!nullable.HasValue) return DBNull.Value;
            else return nullable.Value;
        }

        /// <summary>
        /// Same as dr.Field&lt;T&gt;(columnName) except this handles DBNulls and invalid 
        /// </summary>
        public static T FieldOrDefault<T>(this DataRow dr, string columnName, T defval)
        {
            if (dr[columnName] == DBNull.Value)
                return defval;
            else
                return dr.Field<T>(columnName);
        }

        ///<summary>
        /// Sets a DataRow value if the column exists.
        ///</summary>
        public static void SetFieldIfExists<T>(this DataRow dr, string column, T value)
        {
            if (dr != null && dr.Table.Columns.Contains(column))
                dr[column] = value;
        }

        /// <summary>
        /// Sets the AllowDBNull property of a column to true if the column exists in the table.
        /// </summary>
        public static void AllowNulls(this DataTable dt, string columnName)
        {
            if (dt.Columns.Contains(columnName))
                dt.Columns[columnName].AllowDBNull = true;
        }

        ///<summary>
        /// Returns a string with a maximum length. Any characters after length are truncated but
        /// if the string is shorter than length all characters are returned.
        ///</summary>
        public static string Clip(this string s, int length, string suffix = null)
        {
            var left = Utility.Clip(s, length);

            if (string.IsNullOrEmpty(suffix))
                return left;
            else
            {
                if (left.Length < s.Length)
                    return left + suffix;
                else
                    return left;
            }
        }

        public static T GetRequestValueOrDefault<T>(this IContext context, string key, T defval)
        {
            object obj = context.GetRequestValue(key);
            if (obj == null)
                return defval;
            else
                return Utility.ConvertTo(obj, defval);
        }

        public static DateTime FirstOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1, 0, 0, 0, DateTimeKind.Local);
        }

        public static DateTime LastOfMonth(this DateTime date)
        {
            return date.FirstOfMonth().AddMonths(1).AddMilliseconds(-1);
        }

        public static XmlDocument ToXmlDocument(this XDocument xdoc)
        {
            XmlDocument xmlDocument = new XmlDocument();

            using (var xmlReader = xdoc.CreateReader())
                xmlDocument.Load(xmlReader);

            return xmlDocument;
        }

        public static XDocument ToXDocument(this XmlDocument xdoc)
        {
            using (XmlNodeReader nodeReader = new XmlNodeReader(xdoc))
            {
                nodeReader.MoveToContent();
                return XDocument.Load(nodeReader);
            }
        }
    }
}
