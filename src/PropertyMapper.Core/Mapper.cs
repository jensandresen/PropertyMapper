using System;
using System.Collections.Generic;
using System.Linq;

namespace PropertyMapper
{
    public class Mapper
    {
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

            var propertyBridges = CreatePropertyBridgesFrom(source, destination);

            foreach (var bridge in propertyBridges)
            {
                if (!cfg.ShouldBeIgnored(bridge.DestinationProperty))
                {
                    bridge.CopyValue(source, destination);
                }
            }
        }

        private static IEnumerable<PropertyBridge> CreatePropertyBridgesFrom(object source, object destination)
        {
            var sourceProperties = GetPropertiesFrom(source);
            var destinationProperties = GetPropertiesFrom(destination);

            var analyzer = new SourcePropertiesAnalyzer(sourceProperties);

            foreach (var destinationProperty in destinationProperties)
            {
                var sourceProperty = analyzer.GetMatchFor(destinationProperty);

                if (sourceProperty != null)
                {
                    yield return sourceProperty;
                }
            }
        }

        private static IProperty[] GetPropertiesFrom(object source)
        {
            var targetedType = source.GetType();
            var properties = PropertyHelpers.GetAvailablePropertiesFrom(targetedType);

            return properties;
        }
    }

    public class PropertyBridge
    {
        public PropertyBridge(IProperty source, IProperty destination)
        {
            SourceProperty = source;
            DestinationProperty = destination;
        }

        public IProperty SourceProperty { get; private set; }
        public IProperty DestinationProperty { get; private set; }

        public void CopyValue(object source, object destination)
        {
            var value = GetValueFromInstance(source);
            DestinationProperty.SetValue(destination, value);
        }

        protected virtual object GetValueFromInstance(object source)
        {
            return SourceProperty.GetValue(source);
        }
    }

    public class AssociationPropertyBridge : PropertyBridge
    {
        private readonly IProperty _associationSource;

        public AssociationPropertyBridge(IProperty source, IProperty associationSource, IProperty destination) : base(source, destination)
        {
            _associationSource = associationSource;
        }

        protected override object GetValueFromInstance(object source)
        {
            var association = base.GetValueFromInstance(source);
            return _associationSource.GetValue(association);
        }
    }

    public class SourcePropertiesAnalyzer
    {
        private readonly IProperty[] _properties;
        private readonly IPropertySearchStrategy[] _strategies;

        public SourcePropertiesAnalyzer(IEnumerable<IProperty> properties)
        {
            _properties = properties.ToArray(); // refactor this!

            _strategies = new IPropertySearchStrategy[]
            {
                new DirectNameAndTypeMatchStrategy(_properties),
                new AssociationMatchStrategy(_properties), 
            };
        }

        public PropertyBridge GetMatchFor(IProperty destinationProperty)
        {
            foreach (var strategy in _strategies)
            {
                var matchingBridge = strategy.GetMatchFor(destinationProperty);

                if (matchingBridge != null)
                {
                    return matchingBridge;
                }
            }

            return null;
        }
    }

    public interface IPropertySearchStrategy
    {
        PropertyBridge GetMatchFor(IProperty destinationProperty);
    }

    public abstract class SearchStrategyBase : IPropertySearchStrategy
    {
        protected SearchStrategyBase(IEnumerable<IProperty> properties)
        {
            Properties = properties;
        }

        protected IEnumerable<IProperty> Properties { get; private set; }

        public abstract PropertyBridge GetMatchFor(IProperty destinationProperty);
    }

    public class DirectNameAndTypeMatchStrategy : SearchStrategyBase
    {
        public DirectNameAndTypeMatchStrategy(IEnumerable<IProperty> properties) : base(properties)
        {

        }

        public override PropertyBridge GetMatchFor(IProperty destinationProperty)
        {
            foreach (var sourceProperty in Properties)
            {
                var isMatch = PropertyHelpers.IsMatch(sourceProperty, destinationProperty);

                if (isMatch)
                {
                    return new PropertyBridge(sourceProperty, destinationProperty);
                }
            }

            return null;
        }
    }

    public class AssociationMatchStrategy : SearchStrategyBase
    {
        public AssociationMatchStrategy(IEnumerable<IProperty> properties)
            : base(properties)
        {

        }

        public override PropertyBridge GetMatchFor(IProperty destinationProperty)
        {
            var names = StringHelper
                .SplitByPascalCasing(destinationProperty.Name)
                .ToArray();

            if (names.Length != 2)
            {
                return null;
            }

            var expectedSourceAssociationPropertyName = names[0];
            var expectedSourcePropertyName = names[1];

            foreach (var sourceProperty in Properties)
            {
                if (sourceProperty.Name == expectedSourceAssociationPropertyName)
                {
                    var temp = PropertyHelpers.GetAvailablePropertiesFrom(sourceProperty.Type);

                    foreach (var property in temp)
                    {
                        var isMatch = PropertyHelpers.IsMatch(property.Type, property.Name, destinationProperty.Type, expectedSourcePropertyName);

                        if (isMatch)
                        {
                            return new AssociationPropertyBridge(sourceProperty, property, destinationProperty);
                        }
                    }
                }
            }

            return null;
        }
    }

}