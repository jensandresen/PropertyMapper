using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PropertyMapper.Tests
{
    public class Mapper
    {
        private static readonly IDictionary<Type, PropertyInfo[]> _propertyInfoes = new Dictionary<Type, PropertyInfo[]>();

        public static void Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            Map(source, destination, null);
        }

        private static void CopyPropertyValue(object source, PropertyInfo sourceProperty, object destination, PropertyInfo destinationProperty)
        {
            var value = sourceProperty.GetValue(source, null);
            destinationProperty.SetValue(destination, value, null);
        }

        private static PropertyInfo[] GetPropertiesFrom(object source)
        {
            var targetedType = source.GetType();

            if (_propertyInfoes.ContainsKey(targetedType))
            {
                return _propertyInfoes[targetedType];
            }

            var properties = targetedType
                .GetProperties()
                .Where(p => p.GetGetMethod(false) != null)
                .Where(p => p.GetSetMethod(false) != null)
                .ToArray();

            _propertyInfoes.Add(targetedType, properties);

            return properties;
        }

        private static bool IsMatch(PropertyInfo first, PropertyInfo second)
        {
            return PropertyHelpers.IsMatch(first, second);
        }

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
                        CopyPropertyValue(source, sourceProperty, destination, destinationProperty);
                    }
                }
            }
        }
    }
}