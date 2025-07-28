using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using PDS.Core.Api.Utils;
using PDS.SpaceNew.Common;

namespace PDS.SpaceNew.Common
{
    /// <summary>
    /// Utility methods used for aggreagtion.
    /// </summary>
    public static class SpaceAggregationUtilsLocally
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
            if (s == null)
                return double.NaN;
            else if (s == ".")
                return '.';
            return double.Parse(s.Replace(',', '.'), CultureInfo.InvariantCulture);
        }

        public static double ParseDoubleOrDefault(string valueString)
        {
            return double.TryParse(valueString?.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double result) ? result : double.NaN;
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

        //
        // Summary:
        //     Site independend character replacements.
        private static readonly Dictionary<string, string> GeneralReplaceMap = new Dictionary<string, string>
        {
            { "ä", "ae" },
            { "ö", "oe" },
            { "ü", "ue" },
            { "Ö", "Oe" },
            { "Ä", "Ae" },
            { "Ü", "Ue" },
            { "ß", "ss" },
            { "°", "degree" },
            { "²", "'square" },
            { "³", "'cubic" },
            { "\\", " " },
            { "µ", "micro_" },
            { "&", " " },
            { "#", " " },
            { "\u00b4", "" },
            {
                char.ConvertFromUtf32(34),
                char.ConvertFromUtf32(39)
            },
            { "/", " " },
            { ";", " " },
            {
                char.ConvertFromUtf32(8),
                " "
            },
            {
                char.ConvertFromUtf32(12),
                " "
            },
            {
                char.ConvertFromUtf32(10),
                " "
            },
            {
                char.ConvertFromUtf32(13),
                " "
            },
            {
                char.ConvertFromUtf32(9),
                " "
            },
            { "Î", "I" },
            { "Ó", "O" },
            { "Â", "A" },
            { "É", "E" },
            { "Í", "I" },
            { "à", "a" },
            { "è", "e" },
            { "ì", "i" },
            { "ò", "o" },
            { "ù", "u" },
            { "á", "a" },
            { "é", "e" },
            { "í", "i" },
            { "ó", "o" },
            { "ú", "u" },
            { "ő", "o" },
            { "μ", "micro_" },
            { "Á", "A" },
            { "ű", "u" }
        };

        //
        // Summary:
        //     Maps the site key to site specific character replacements.
        private static readonly Dictionary<string, Dictionary<string, string>> SiteReplaceMap = new Dictionary<string, Dictionary<string, string>> {
        {
            "CEG",
            new Dictionary<string, string>
            {
                { "ö", "o" },
                { "ä", "a" },
                { "ü", "u" },
                { "Ö", "O" },
                { "Ä", "A" },
                { "Ü", "U" }
            }
        } };

        //
        // Summary:
        //     Normalizes special characters in input depending on site.
        //
        // Parameters:
        //   input:
        //     The input text.
        //
        //   siteKey:
        //     The site key.
        //
        // Returns:
        //     The normalized text or null if input is null.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     if siteKey is null
        public static string Normalize(string input, [NotNull] string siteKey)
        {
            Ensure.NotNull(siteKey, "siteKey");
            if (input == null)
            {
                return null;
            }

            if (SiteReplaceMap.ContainsKey(siteKey))
            {
                input = Replace(input, SiteReplaceMap[siteKey]);
            }

            return Replace(input, GeneralReplaceMap);
        }

        private static string Replace(string text, Dictionary<string, string> replaceMap)
        {
            StringBuilder stringBuilder = new StringBuilder(text);
            foreach (KeyValuePair<string, string> item in replaceMap)
            {
                stringBuilder.Replace(item.Key, item.Value);
            }

            return stringBuilder.ToString();
        }
    }
}
