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

            var propertyPairs = CreatePropertyPairsFrom(source, destination);

            foreach (var pair in propertyPairs)
            {
                if (!cfg.ShouldBeIgnored(pair.DestinationProperty))
                {
                    pair.CopyValue(source, destination);
                }
            }
        }

        private static IEnumerable<PropertyPair> CreatePropertyPairsFrom(object source, object destination)
        {
            var sourceProperties = GetPropertiesFrom(source);
            var destinationProperties = GetPropertiesFrom(destination);

            var analyzer = new SourcePropertiesAnalyzer(sourceProperties);

            foreach (var destinationProperty in destinationProperties)
            {
                var sourceProperty = analyzer.GetMatchFor(destinationProperty);

                if (sourceProperty != null)
                {
                    yield return new PropertyPair(sourceProperty, destinationProperty);
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
    }

    public class PropertyPair
    {
        public PropertyPair(PropertyInfo source, PropertyInfo destination)
        {
            SourceProperty = source;
            DestinationProperty = destination;
        }

        public PropertyInfo SourceProperty { get; private set; }
        public PropertyInfo DestinationProperty { get; private set; }

        public void CopyValue(object source, object destination)
        {
            PropertyHelpers.CopyPropertyValue(source, SourceProperty, destination, DestinationProperty);
        }
    }

    public class SourcePropertiesAnalyzer
    {
        private readonly PropertyInfo[] _properties;
        private readonly IPropertySearchStrategy[] _strategies;

        public SourcePropertiesAnalyzer(IEnumerable<PropertyInfo> properties)
        {
            _properties = properties.ToArray(); // refactor this!

            _strategies = new IPropertySearchStrategy[]
            {
                new DirectNameAndTypeMatchStrategy(_properties), 
            };
        }

        public PropertyInfo GetMatchFor(PropertyInfo destinationProperty)
        {
            foreach (var strategy in _strategies)
            {
                var matchingProperty = strategy.GetMatchFor(destinationProperty);

                if (matchingProperty != null)
                {
                    return matchingProperty;
                }
            }

            return null;
        }
    }

    public interface IPropertySearchStrategy
    {
        PropertyInfo GetMatchFor(PropertyInfo destinationProperty);
    }

    public abstract class SearchStrategyBase : IPropertySearchStrategy
    {
        protected SearchStrategyBase(IEnumerable<PropertyInfo> properties)
        {
            Properties = properties;
        }

        protected IEnumerable<PropertyInfo> Properties { get; private set; }

        public abstract PropertyInfo GetMatchFor(PropertyInfo destinationProperty);
    }

    public class DirectNameAndTypeMatchStrategy : SearchStrategyBase
    {
        public DirectNameAndTypeMatchStrategy(IEnumerable<PropertyInfo> properties) : base(properties)
        {

        }

        public override PropertyInfo GetMatchFor(PropertyInfo destinationProperty)
        {
            foreach (var sourceProperty in Properties)
            {
                var isMatch = PropertyHelpers.IsMatch(sourceProperty, destinationProperty);

                if (isMatch)
                {
                    return sourceProperty;
                }
            }

            return null;
        }
    }
}