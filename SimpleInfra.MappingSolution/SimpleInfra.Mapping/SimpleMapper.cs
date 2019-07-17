namespace SimpleInfra.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   A general simple mapper class. </summary>
    ///
    /// <remarks>   Msacli, 22.04.2019. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    public static class SimpleMapper
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   A TSource extension method that maps the given source. </summary>
        ///
        /// <remarks>   Msacli, 22.04.2019. </remarks>
        ///
        /// <typeparam name="TSource">  Type of the source. </typeparam>
        /// <typeparam name="TDest">    Type of the destination. </typeparam>
        /// <param name="source">   The source to act on. </param>
        /// <param name="notUseCache">if true will be used Cache</param>
        ///
        /// <returns>   A TDest. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public static TDest Map<TSource, TDest>(TSource source, bool notUseCache = false)
            where TSource : class
            where TDest : class, new()
        {
            if (source == null)
                return null;

            Type typeDest = typeof(TDest);
            Type typeSource = typeof(TSource);

            var list = new List<string>();
            list = (notUseCache ?
                SimpleTypeHelper.GetSameProperties(typeDest, typeSource)
                : SimpleTypeHelper.GetSamePropertiesFromDict(typeDest, typeSource)) ?? new List<string>();

            var dest = Activator.CreateInstance<TDest>();

            SetInstanceValues(source, dest, list);

            return dest;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   A TSource extension method that maps the given source. </summary>
        ///
        /// <remarks>   Msacli, 22.04.2019. </remarks>
        ///
        /// <typeparam name="TSource">  Type of the source. </typeparam>
        /// <typeparam name="TDest">    Type of the destination. </typeparam>
        /// <param name="sourceList">   The source List. </param>
        /// <param name="notUseCache"></param>
        ///
        /// <returns>   A TDest. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public static List<TDest> MapList<TSource, TDest>(List<TSource> sourceList, bool notUseCache = false)
            where TSource : class
            where TDest : class, new()
        {
            var resultList = new List<TDest>();

            if (sourceList == null || sourceList.Count < 1)
                return resultList;

            Type typeDest = typeof(TDest);
            Type typeSource = typeof(TSource);

            var list = new List<string>();

            list = (notUseCache ?
                SimpleTypeHelper.GetSameProperties(typeDest, typeSource)
                : SimpleTypeHelper.GetSamePropertiesFromDict(typeDest, typeSource)) ?? new List<string>();

            sourceList.ForEach(source =>
            {
                var dest = Activator.CreateInstance<TDest>();

                SetInstanceValues(source, dest, list);

                resultList.Add(dest);
            });

            return resultList;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   A TSource extension method that maps the given source. </summary>
        ///
        /// <remarks>   Msacli, 22.04.2019. </remarks>
        ///
        /// <typeparam name="TSource">  Type of the source. </typeparam>
        /// <typeparam name="TDest">    Type of the destination. </typeparam>
        /// <param name="propertyMap">   Property Map string, like SourceProp1:DestProp1;SourceProp2:DestProp2;SourceProp3:DestProp3. </param>
        /// <param name="source">   The source to act on. </param>
        ///
        /// <returns>   A TDest. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public static TDest Map<TSource, TDest>(TSource source, string propertyMap)
            where TSource : class
            where TDest : class, new()
        {
            if (string.IsNullOrWhiteSpace(propertyMap))
                return Map<TSource, TDest>(source, notUseCache: false);

            if (source == null)
                return null;

            Type typeDest = typeof(TDest);
            Type typeSource = typeof(TSource);
            var dictionary = new Dictionary<string, string>();

            dictionary = SimpleTypeHelper.GetKeyValues(propertyMap,
                InternalAppValues.FirstDelimiter, InternalAppValues.SecondDelimiter) ?? new Dictionary<string, string>();

            var dest = Activator.CreateInstance<TDest>();

            SetInstanceValues(source, dest, dictionary);

            return dest;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   A TSource extension method that maps the given source. </summary>
        ///
        /// <remarks>   Msacli, 22.04.2019. </remarks>
        ///
        /// <typeparam name="TSource">  Type of the source. </typeparam>
        /// <typeparam name="TDest">    Type of the destination. </typeparam>
        /// <param name="propertyMap">   Property Map string, like SourceProp1:DestProp1;SourceProp2:DestProp2;SourceProp3:DestProp3. </param>
        /// <param name="sourceList">   The source List. </param>
        ///
        /// <returns>   A TDest. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public static List<TDest> Map<TSource, TDest>(List<TSource> sourceList, string propertyMap)
            where TSource : class
            where TDest : class, new()
        {
            if (string.IsNullOrWhiteSpace(propertyMap))
                return MapList<TSource, TDest>(sourceList, notUseCache: false);

            var resultList = new List<TDest>();

            if (sourceList == null || sourceList.Count < 1)
                return resultList;

            Type typeDest = typeof(TDest);
            Type typeSource = typeof(TSource);
            var dictionary = new Dictionary<string, string>();

            dictionary =
                SimpleTypeHelper.GetKeyValues(propertyMap,
                InternalAppValues.FirstDelimiter, InternalAppValues.SecondDelimiter)
                ?? new Dictionary<string, string>();

            sourceList.ForEach(source =>
            {
                var dest = Activator.CreateInstance<TDest>();

                SetInstanceValues(source, dest, dictionary);
                resultList.Add(dest);
            });

            return resultList;
        }

        /// <summary>
        /// Map Property Values to another type instance.
        /// </summary>
        /// <typeparam name="TSource">Source Generic Type</typeparam>
        /// <typeparam name="TDest">Destination Generic Type</typeparam>
        /// <param name="source">Source generic type instance</param>
        /// <param name="instance">Destination generic type instance</param>
        /// <param name="notUseCache"></param>
        public static void MapTo<TSource, TDest>(TSource source,
            TDest instance, bool notUseCache = false)
            where TSource : class
            where TDest : class
        {
            if (source == null)
                return;

            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            Type typeDest = typeof(TDest);
            Type typeSource = typeof(TSource);

            var list = new List<string>();
            list = (notUseCache ?
                SimpleTypeHelper.GetSameProperties(typeDest, typeSource)
                : SimpleTypeHelper.GetSamePropertiesFromDict(typeDest, typeSource)) ?? new List<string>();

            SetInstanceValues(source, instance, list);
        }

        private static void SetInstanceValues<TSource, TDest>(TSource source, TDest dest,
            List<string> propertyList)
            where TSource : class
            where TDest : class
        {
            Type typeDest = typeof(TDest);
            Type typeSource = typeof(TSource);
            PropertyInfo propSource;
            PropertyInfo propDest;

            propertyList.ForEach(q =>
            {
                propSource = typeSource.GetProperty(q);
                propDest = typeDest.GetProperty(q);

                var value = propSource.GetValue(source, null);

                if (IsNullOrDefault(value) == false)
                    propDest.SetValue(dest, value);
            });
        }

        private static void SetInstanceValues<TSource, TDest>(TSource source, TDest dest,
            Dictionary<string, string> propertyList)
            where TSource : class
            where TDest : class
        {
            if (propertyList == null || propertyList.Count < 1)
                return;

            Type typeDest = typeof(TDest);
            Type typeSource = typeof(TSource);
            PropertyInfo propSource;
            PropertyInfo propDest;

            propertyList.Keys.ToList().ForEach(q =>
            {
                propSource = typeSource.GetProperty(q);
                propDest = typeDest.GetProperty(propertyList[q]);

                var value = propSource.GetValue(source, null);

                if (IsNullOrDefault(value) == false)
                    propDest.SetValue(dest, value);
            });
        }

        private static bool IsNullOrDefault(object value)
        {
            var r = value == null || value == (object)DBNull.Value;
            return r;
        }
    }
}