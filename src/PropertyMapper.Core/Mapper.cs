using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PropertyMapper
{
    public class Mapper
    {
        private static readonly IDictionary<Type, PropertyInfo[]> _propertyInfoes = new Dictionary<Type, PropertyInfo[]>();

        /// <summary>
        /// Copy the value of all properties that has read and write access on both source and destination.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="source">The instance to copy property values from.</param>
        /// <param name="destination">The instance where the property values are copied to.</param>
        public static void Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            Map(source, destination, null);
        }

        /// <summary>
        /// Copy the value of all properties that has read and write access on both source and destination. Adjust mapping configuration
        /// to enable a more customized mapping.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="source">The instance to copy property values from.</param>
        /// <param name="destination">The instance where the property values are copied to.</param>
        /// <param name="configurator">The configuration instance that enables customization.</param>
        public static void Map<TSource, TDestination>(TSource source, TDestination destination, Action<IConfiguration<TSource, TDestination>> configurator)
        {
            var cfg = new Configuration<TSource, TDestination>();
            if (configurator != null)
            {
                configurator(cfg);
            }

            var sourceProperties = GetPropertiesFrom(source);
            var destinationProperties = GetPropertiesFrom(destination);

            foreach (var destinationProperty in destinationProperties)
            {
                if (!cfg.ShouldBeIgnored(destinationProperty))
                {
                    var sourceProperty = sourceProperties.SingleOrDefault(info => IsMatch(info, destinationProperty));

                    if (sourceProperty != null)
                    {
                        PropertyHelpers.CopyPropertyValue(source, sourceProperty, destination, destinationProperty);
                    }
                }
            }
        }

        private static PropertyInfo[] GetPropertiesFrom(object source)
        {
            var targetedType = source.GetType();

            if (_propertyInfoes.ContainsKey(targetedType))
            {
                return _propertyInfoes[targetedType];
            }

            var properties = PropertyHelpers.GetAvailablePropertiesFrom(targetedType);
            _propertyInfoes.Add(targetedType, properties);

            return properties;
        }

        private static bool IsMatch(PropertyInfo first, PropertyInfo second)
        {
            return PropertyHelpers.IsMatch(first, second);
        }
    }
}