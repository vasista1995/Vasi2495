using System;
using System.Collections.Generic;
using System.Globalization;

namespace PDS.SpaceNew.Common
{
    public static class ListExtensions
    {
        public static List<T> ConvertListOrThrow<T>(this List<string> sourceList)
        {
            var targetList = new List<T>();
            foreach (var item in sourceList)
            {
                try
                {
                    var convertedItem = (T) Convert.ChangeType(item, typeof(T), CultureInfo.InvariantCulture);
                    targetList.Add(convertedItem);
                }
                catch (FormatException ex)
                {
                    string errorMessage = $"Error converting '{item}' to {typeof(T).Name}: Invalid format.";
                    throw new FormatException(errorMessage, ex);
                }
                catch (InvalidCastException ex)
                {
                    string errorMessage = $"Conversion from string to {typeof(T).Name} is not supported.";
                    throw new InvalidCastException(errorMessage, ex);
                }
                catch (OverflowException ex)
                {
                    string errorMessage = $"Error converting '{item}' to {typeof(T).Name}: Value is out of range.";
                    throw new OverflowException(errorMessage, ex);
                }
                catch (ArgumentNullException ex)
                {
                    string errorMessage = "Null values cannot be converted.";
                    throw new ArgumentNullException(errorMessage, ex);
                }
                catch (Exception ex)
                {
                    throw new ArgumentNullException($"Error converting value '{item}': {ex.Message}", ex);
                }
            }

            return targetList;
        }
    }
}
