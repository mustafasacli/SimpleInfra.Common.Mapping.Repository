using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace SimpleInfra.Mapping
{
    internal static class SimpleTypeHelper
    {
        private static ConcurrentDictionary<string, string> dictionary = null;

        static SimpleTypeHelper()
        {
            dictionary = new ConcurrentDictionary<string, string>();
        }

        /// <summary>
        /// Gets common Properties of two types.
        /// </summary>
        /// <param name="type1">First type</param>
        /// <param name="type2">Second type</param>
        /// <returns>returns string list</returns>
        internal static List<string> GetSamePropertiesFromDict(Type type1, Type type2)
        {
            var properties = new List<string>();

            var key1 = string.Format("{0}_{1}", type1.FullName, type2.FullName);
            var key2 = string.Format("{0}_{1}", type2.FullName, type1.FullName);

            var propertiesAsString = string.Empty;
            if (dictionary.TryGetValue(key1, out propertiesAsString))
            {
                if (string.IsNullOrWhiteSpace(propertiesAsString))
                {
                    properties = GetSameProperties(type1, type2);
                    propertiesAsString = string.Join(InternalAppValues.JoinString, properties);
                    dictionary[key1] = propertiesAsString;
                    dictionary[key2] = propertiesAsString;
                }
                else
                {
                    properties =
                    propertiesAsString
                    .Split(new[] { InternalAppValues.JoinChar }, StringSplitOptions.RemoveEmptyEntries)
                    .ToList();
                }
            }
            else if (dictionary.TryGetValue(key2, out propertiesAsString))
            {
                if (string.IsNullOrWhiteSpace(propertiesAsString))
                {
                    properties = GetSameProperties(type1, type2);
                    propertiesAsString = string.Join(InternalAppValues.JoinString, properties);
                    dictionary[key1] = propertiesAsString;
                    dictionary[key2] = propertiesAsString;
                }
                else
                {
                    properties =
                    propertiesAsString
                    .Split(new[] { InternalAppValues.JoinChar }, StringSplitOptions.RemoveEmptyEntries)
                    .ToList();
                }
            }
            else
            {
                properties = GetSameProperties(type1, type2);
                propertiesAsString = string.Join(InternalAppValues.JoinString, properties);
                dictionary[key1] = propertiesAsString;
                dictionary[key2] = propertiesAsString;
            }

            return properties;
        }

        /// <summary>
        /// Gets common Properties of two types.
        /// </summary>
        /// <param name="type1">First type</param>
        /// <param name="type2">Second type</param>
        /// <returns>returns string list</returns>
        internal static List<string> GetSameProperties(Type type1, Type type2)
        {
            var list = new List<string>();
            var dictionary = new Dictionary<string, Type>();

            type1.GetProperties()
                .Where(q => q.CanRead && q.CanWrite)
                .ToList()
                .ForEach(
                q =>
                {
                    dictionary.Add(q.Name, Nullable.GetUnderlyingType(q.PropertyType) ?? q.PropertyType);
                });

            list = type2
                .GetProperties()
                .Where(q => dictionary.ContainsKey(q.Name) && q.CanRead && q.CanWrite)
                .Where(q => (Nullable.GetUnderlyingType(q.PropertyType) ?? q.PropertyType) == dictionary[q.Name])
                .Select(q => q.Name)
                .ToList() ?? new List<string>();

            return list;
        }

        internal static Dictionary<string, string> GetKeyValues(string splitString, char firstDelimiter, char secondDelimiter)
        {
            var keyValues = new Dictionary<string, string>();

            if (string.IsNullOrWhiteSpace(splitString))
                return keyValues;

            var keyVals = splitString.Split(new char[] { firstDelimiter }, StringSplitOptions.RemoveEmptyEntries) ?? new string[] { };

            foreach (var item in keyVals)
            {
                var dictItems = item.Split(new char[] { secondDelimiter }, StringSplitOptions.RemoveEmptyEntries) ?? new string[] { };

                if (dictItems.Length != 2)
                    continue;

                keyValues[dictItems[0]] = dictItems[1];
            }

            return keyValues;
        }
    }
}