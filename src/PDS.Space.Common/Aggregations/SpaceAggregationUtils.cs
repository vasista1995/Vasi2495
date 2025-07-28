using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using PDS.Core.Api.Utils;

namespace PDS.Space.Common.Aggregations
{
    /// <summary>
    /// Utility methods used for aggreagtion.
    /// </summary>
    public static class SpaceAggregationUtils
    {
        /// <summary>
        /// if equal it takes the lot time
        /// if not equal it takes the minimum of both times
        /// </summary>
        /// <param name="padsTime"> the time from mongodb extracted document</param>
        /// <param name="e4aTime"> the time from e4a document from the queue</param>
        /// <returns> minimum value of "padsTime" & "e4aTime" </returns>
        public static DateTime MinDate(DateTime padsTime, DateTime e4aTime)
        {
            return padsTime < e4aTime ? padsTime : e4aTime;
        }

        public static double DoubleParse(string s)
        {
            return double.Parse(s.Replace(',', '.'), CultureInfo.InvariantCulture);
        }

        public static double ParseDoubleOrDefault(string valueString)
        {
            return double.TryParse(valueString?.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out var result) ? result : double.NaN;
        }

        /// <summary>
        /// if equal it takes the lot time
        /// if not equal it takes the maximum of both times
        /// </summary>
        /// <param name="padsTime"> the time from mongodb extracted document</param>
        /// <param name="e4aTime"> the time from e4a document from the queue</param>
        /// <returns> maximum value of "padsTime" & "e4aTime" </returns>
        public static DateTime MaxDate(DateTime padsTime, DateTime e4aTime)
        {
            return padsTime > e4aTime ? padsTime : e4aTime;
        }


        /// <summary>
        /// This method checks if the given limits are equal or not
        /// </summary>
        /// <param name="padsLimit">Limit from modb extracted document</param>
        /// <param name="e4aLimit">Limit from e4a document from the queue</param>
        /// <returns>"padsLimit" or "null" for equal and not equal limits respectively</returns>
        public static double? UpdateLimit(double? padsLimit, double? e4aLimit)
        {
            return padsLimit.NearlyEqual(e4aLimit) ? padsLimit : null;
        }


        /// <summary>
        /// This method checks if the given limitenabled values are equal or not
        /// </summary>
        /// <param name="padsLimitEnabled">Limit enabled value from modb extracted document</param>
        /// <param name="e4aLimitEnabled">Limit enabled value from e4a document from the queue</param>
        /// <returns>"padsLimitEnabled" or "null" for equal and not equal limitEnabled values
        /// respectively</returns>
        public static string UpdateLimitEnabled(string padsLimitEnabled, string e4aLimitEnabled)
        {
            return padsLimitEnabled == e4aLimitEnabled ? padsLimitEnabled : null;
        }


        /// <summary>
        /// This method checks if the ambiguity of the given limits
        /// </summary>
        /// <param name="padsLimit">Limit from modb extracted document</param>
        /// <param name="e4aLimit">Limit from e4a document from the queue</param>
        /// <returns>true or false based on not equal and equal limit values</returns>
        public static bool IsLimitAmbigous(double? padsLimit, double? e4aLimit)
        {
            return !padsLimit.NearlyEqual(e4aLimit);
        }


        /// <summary>
        /// checks if the given string values are equal or not
        /// </summary>
        /// <param name="padsString">string from modb extracted document</param>
        /// <param name="e4aString">string from e4a document from the queue</param>
        /// <returns> "padsString" or "padsString e4aString" for equal and notequal
        /// given strings respectively</returns>
        public static string JoinStrings(string padsString, string e4aString)
        {
            if (padsString == null || e4aString == null)
            {
                if (padsString == null)
                    return e4aString;
                else
                {
                    return padsString;
                }
            }
            else
            {
                if (padsString != e4aString)
                {
                    var values = new List<string>();
                    values.AddRange(padsString.Split(", "));
                    values.AddRange(e4aString.Split(", "));
                    return string.Join(", ", values.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct());
                }
                else
                    return padsString;
            }
        }
    }
}
