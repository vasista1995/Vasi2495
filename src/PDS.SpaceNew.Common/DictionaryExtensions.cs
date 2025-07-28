using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace PDS.SpaceNew.Common
{
    public static class DictionaryExtensions
    {
        public static TValue GetValueOrThrow<TValue>(this IDictionary<string, TValue> dictionary, string key)
        {
            string keyInDictionary = dictionary.Keys.FirstOrDefault(k => string.Equals(k, key, StringComparison.InvariantCultureIgnoreCase));
            if (keyInDictionary != null)
            {
                return dictionary[keyInDictionary];
            }
            else
            {
                throw new KeyNotFoundException($"Key '{key}' not found in the dictionary");
            }
        }

        public static TValue GetValueOrDefault<TValue>(this IDictionary<string, TValue> dictionary, string key)
        {
            string keyInDictionary = dictionary.Keys.FirstOrDefault(k => string.Equals(k, key, StringComparison.InvariantCultureIgnoreCase));
            if (keyInDictionary != null)
            {
                return dictionary[keyInDictionary];
            }
            else
            {
                return default(TValue);
            }
        }

        public static IDictionary<string, object> ToDictionary(this object obj)
        {
            var dictionary = new Dictionary<string, object>();

            if (obj != null)
            {
                Type type = obj.GetType();
                PropertyInfo[] properties = type.GetProperties();

                foreach (PropertyInfo property in properties)
                {
                    dictionary.Add(property.Name, property.GetValue(obj));
                }
            }

            return dictionary;
        }
    }
}
