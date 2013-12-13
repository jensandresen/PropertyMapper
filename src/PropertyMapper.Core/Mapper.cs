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

            var propertyBridges = GetPropertyBridgesFor(source, destination);

            foreach (var bridge in propertyBridges)
            {
                if (!cfg.ShouldBeIgnored(bridge.DestinationProperty))
                {
                    bridge.CopyValue(source, destination);
                }
            }
        }

        private static IEnumerable<PropertyBridge> GetPropertyBridgesFor(object source, object destination)
        {
            var sourcePropertyRepository = new InstancePropertyRepository(source);
            var analyzer = new SourcePropertiesAnalyzer(sourcePropertyRepository);
            var destinationProperties = PropertyHelpers.GetAvailablePropertiesFrom(destination);

            foreach (var destinationProperty in destinationProperties)
            {
                var propertyBridge = analyzer.GetMatchFor(destinationProperty);

                if (propertyBridge != null)
                {
                    yield return propertyBridge;
                }
            }
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
            var value = GetValueFromSourceInstance(source);
            SetValueOnDestinationInstance(destination, value);
        }

        protected virtual void SetValueOnDestinationInstance(object destination, object value)
        {
            DestinationProperty.SetValue(destination, value);
        }

        protected virtual object GetValueFromSourceInstance(object source)
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

        protected override object GetValueFromSourceInstance(object source)
        {
            var association = base.GetValueFromSourceInstance(source);
            return _associationSource.GetValue(association);
        }
    }

    public class SourcePropertiesAnalyzer
    {
        private readonly IPropertySearchStrategy[] _strategies;

        public SourcePropertiesAnalyzer(IPropertyRepository sourcePropertyRepository)
        {
            _strategies = new IPropertySearchStrategy[]
            {
                new DirectNameAndTypeMatchStrategy(sourcePropertyRepository),
                new AssociationMatchStrategy(sourcePropertyRepository), 
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
        private readonly IPropertyRepository _propertyRepository;

        protected SearchStrategyBase(IPropertyRepository propertyRepository)
        {
            _propertyRepository = propertyRepository;
        }

        protected IPropertyRepository PropertyRepository
        {
            get { return _propertyRepository; }
        }

        public abstract PropertyBridge GetMatchFor(IProperty destinationProperty);
    }

    public class DirectNameAndTypeMatchStrategy : SearchStrategyBase
    {
        public DirectNameAndTypeMatchStrategy(IPropertyRepository propertyRepository) : base(propertyRepository)
        {

        }

        public override PropertyBridge GetMatchFor(IProperty destinationProperty)
        {
            foreach (var sourceProperty in PropertyRepository.GetAll())
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
        public AssociationMatchStrategy(IPropertyRepository propertyRepository) : base(propertyRepository)
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

            foreach (var associationProperty in PropertyRepository.GetAll())
            {
                if (associationProperty.Name == expectedSourceAssociationPropertyName)
                {
                    var sourceProperties = PropertyHelpers.GetAvailablePropertiesFrom(associationProperty.Type);

                    foreach (var sourceProperty in sourceProperties)
                    {
                        var isMatch = PropertyHelpers.IsMatch(sourceProperty.Type, sourceProperty.Name, destinationProperty.Type, expectedSourcePropertyName);

                        if (isMatch)
                        {
                            return new AssociationPropertyBridge(associationProperty, sourceProperty, destinationProperty);
                        }
                    }
                }
            }

            return null;
        }
    }

    public interface IPropertyRepository
    {
        IEnumerable<IProperty> GetAll();
    }

    public class InstancePropertyRepository : IPropertyRepository
    {
        private readonly object _sourceInstance;

        public InstancePropertyRepository(object sourceInstance)
        {
            _sourceInstance = sourceInstance;
        }

        public IEnumerable<IProperty> GetAll()
        {
            return PropertyHelpers.GetAvailablePropertiesFrom(_sourceInstance);
        }
    }

    public class TypePropertyRepository : IPropertyRepository
    {
        private readonly Type _sourceType;

        public TypePropertyRepository(Type sourceType)
        {
            _sourceType = sourceType;
        }

        public IEnumerable<IProperty> GetAll()
        {
            return PropertyHelpers.GetAvailablePropertiesFrom(_sourceType);
        }
    }
}