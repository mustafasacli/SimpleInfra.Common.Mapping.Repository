using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace SimpleInfra.Mapping
{
    internal static class SimpleTypeHelper
    {
        private static readonly ConcurrentDictionary<string, string> dictionary =
            new ConcurrentDictionary<string, string>();

        /// <summary>
        /// Gets common Properties of two types.
        /// </summary>
        /// <param name="type1">First type</param>
        /// <param name="type2">Second type</param>
        /// <param name="useOnlySimpleTypes">if it is true uses only simple types, else includes complex types(for example class).</param>
        /// <returns>returns string list</returns>
        internal static List<string> GetSamePropertiesFromDict(Type type1, Type type2, bool useOnlySimpleTypes = true)
        {
            List<string> properties;

            var key1 = string.Format("{0}_{1}", type1.FullName, type2.FullName);
            var key2 = string.Format("{0}_{1}", type2.FullName, type1.FullName);

            string propertiesAsString;
            if (dictionary.TryGetValue(key1, out propertiesAsString))
            {
                if (string.IsNullOrWhiteSpace(propertiesAsString))
                {
                    properties = GetSameProperties(type1, type2, useOnlySimpleTypes);
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
                    properties = GetSameProperties(type1, type2, useOnlySimpleTypes);
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
                properties = GetSameProperties(type1, type2, useOnlySimpleTypes);
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
        /// <param name="useOnlySimpleTypes">if it is true uses only simple types, else includes complex types(for example class).</param>
        /// <returns>returns string list</returns>
        internal static List<string> GetSameProperties(Type type1, Type type2, bool useOnlySimpleTypes = true)
        {
            List<string> list;
            var propertyTypes = new Dictionary<string, Type>();

            type1.GetProperties()
                .Where(q => q.CanRead && q.CanWrite &&
                (!useOnlySimpleTypes || IsSimpleTypeV2(Nullable.GetUnderlyingType(q.PropertyType) ?? q.PropertyType)))
                .ToList()
                .ForEach(
                q =>
                {
                    propertyTypes.Add(q.Name, Nullable.GetUnderlyingType(q.PropertyType) ?? q.PropertyType);
                });

            list = type2
                .GetProperties()
                .Where(q => propertyTypes.ContainsKey(q.Name) && q.CanRead && q.CanWrite)
                .Where(q => (Nullable.GetUnderlyingType(q.PropertyType) ?? q.PropertyType) == propertyTypes[q.Name])
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

        /// <summary>
        /// checks type is SimpleType.
        /// </summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>True if it succeeds, false if it fails.</returns>
        internal static bool IsSimpleTypeV2(this Type type)
        {
            return
                type.IsPrimitive ||
                new Type[] {
            typeof(byte[]),
            typeof(Enum),
            typeof(string),
            typeof(decimal),
            typeof(DateTime),
            typeof(DateTimeOffset),
            typeof(TimeSpan),
            typeof(XmlDocument),
            typeof(XmlNode),
            typeof(Guid)
                }.Contains(type) ||
                Convert.GetTypeCode(type) != TypeCode.Object ||
                (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) && IsSimpleTypeV2(type.GetGenericArguments()[0]))
                ;
        }
    }
}