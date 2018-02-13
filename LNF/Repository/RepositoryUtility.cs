using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Repository
{
    /// <summary>
    /// Helper methods for common functionality related to Repositories
    /// </summary>
    public static class RepositoryUtility
    {
        /// <summary>
        /// Converts an object to the specified type
        /// </summary>
        /// <typeparam name="T">The expected return type</typeparam>
        /// <param name="obj">The object to convert</param>
        /// <param name="defval">The default value to return if conversion fails for any reason</param>
        /// <returns>A value of type T</returns>
        [Obsolete("Replaced by LNF.CommonTools.Utility.ConvertTo")]
        public static T ConvertTo<T>(object obj, T defval)
        {
            return CommonTools.Utility.ConvertTo(obj, defval);
        }

        /// <summary>
        /// Tries to convert an object to the specified type
        /// </summary>
        /// <typeparam name="T">The expected return value</typeparam>
        /// <param name="value">The object which will be set to the converted value</param>
        /// <param name="result">A value that indicates if the conversion was successful</param>
        /// <param name="defval">The value to set result to if conversion fails for any reason</param>
        /// <returns>True if the conversion was successful, otherwise false</returns>
        [Obsolete("Replaced by LNF.CommonTools.Utility.TryConvertTo")]
        public static bool TryConvertTo<T>(object value, out T result, T defval)
        {
            return CommonTools.Utility.TryConvertTo(value, out result, defval);
        }

        /// <summary>
        /// Returns the underlying type of the given type. If the given type is nullable the underlying type is returned. Otherwise the given type is returned
        /// </summary>
        /// <param name="t">The specified type</param>
        /// <returns>A type that is the underlying type of the specified type</returns>
        [Obsolete("Replaced by LNF.CommonTools.Utility.GetUnderlyingType")]
        public static Type GetUnderlyingType(Type t)
        {
            return CommonTools.Utility.GetUnderlyingType(t);
        }

        /// <summary>
        /// Determines if two date ranges overlap
        /// </summary>
        /// <param name="startRange1">The start of the first range</param>
        /// <param name="endRange1">The end of the first range</param>
        /// <param name="startRange2">The start of the second range</param>
        /// <param name="endRange2">The end of the second range</param>
        /// <param name="exclusive">Indicates if the overlap is exclusive or inclusive</param>
        /// <returns>True if the ranges overlap, otherwise false</returns>
        public static bool IsOverlapped(DateTime startRange1, DateTime? endRange1, DateTime startRange2, DateTime endRange2, bool exclusive = true)
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

        /// <summary>
        /// Converts an object to a dictionary by relecting property names and values
        /// </summary>
        /// <param name="obj">The specified object</param>
        /// <returns>A dictionary representation of the specified object</returns>
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

        /// <summary>
        /// Indicates if the specified date is the current period
        /// </summary>
        /// <param name="period">The date to check</param>
        /// <returns>True if the date is the current period, otherwise false</returns>
        public static bool IsCurrentPeriod(DateTime period)
        {
            DateTime current = DateTime.Now.Date;
            DateTime sdate = new DateTime(current.Year, current.Month, 1);
            DateTime edate = sdate.AddMonths(1);
            return (period >= sdate && period < edate);
        }
    }
}
