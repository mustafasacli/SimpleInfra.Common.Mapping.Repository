namespace SimpleInfra.Mapping
{
    using System;
    using System.Collections.Generic;
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
        ///
        /// <returns>   A TDest. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public static TDest Map<TSource, TDest>(TSource source)
            where TSource : class
            where TDest : class
        {
            Type typeDest = typeof(TDest);
            Type typeSource = typeof(TSource);

            var list =
                SimpleTypeHelper.GetSamePropertiesFromDict(typeDest, typeSource) ?? new List<string>();
            var dest = Activator.CreateInstance<TDest>();

            SetInstanceValues(source, dest, list);

            return dest;
        }

        private static void SetInstanceValues<TSource, TDest>(TSource source, TDest dest, List<string> propertyList)
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

                if (value != null && value != DBNull.Value)
                    propDest.SetValue(dest, value);
            });
        }

        /// <summary>
        /// Map Property Values to another type instance.
        /// </summary>
        /// <typeparam name="TSource">Source Generic Type</typeparam>
        /// <typeparam name="TDest">Destination Generic Type</typeparam>
        /// <param name="source">Source generic type instance</param>
        /// <param name="instance">Destination generic type instance</param>
        public static void MapTo<TSource, TDest>(TSource source, TDest instance)
            where TSource : class
            where TDest : class
        {
            if (source == null)
                return;

            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            Type typeDest = typeof(TDest);
            Type typeSource = typeof(TSource);
            var list =
                SimpleTypeHelper.GetSamePropertiesFromDict(typeDest, typeSource) ?? new List<string>();

            SetInstanceValues(source, instance, list);
        }
    }
}