using System;
using System.Linq;
using System.Reflection;

namespace PropertyMapper
{
    public static class PropertyHelpers
    {
        public static bool IsMatch(PropertyInfo first, PropertyInfo second)
        {
            return first.PropertyType == second.PropertyType &&
                   first.Name == second.Name;
        }

        public static bool IsMatch(IProperty first, IProperty second)
        {
            return first.Type == second.Type &&
                   first.Name == second.Name;
        }

        public static IProperty[] GetAvailablePropertiesFrom(Type targetedType)
        {
            return targetedType
                .GetProperties()
                .Where(p => p.GetGetMethod(false) != null)
                .Where(p => p.GetSetMethod(false) != null)
                .Select(p => new PropertyInfoAdapter(p))
                .Cast<IProperty>() // todo: remove this!
                .ToArray();
        }

        public static void CopyPropertyValue(object source, PropertyInfo sourceProperty, object destination, PropertyInfo destinationProperty)
        {
            var value = sourceProperty.GetValue(source, null);
            destinationProperty.SetValue(destination, value, null);
        }

        public static void CopyPropertyValue(object source, IProperty sourceProperty, object destination, IProperty destinationProperty)
        {
            var value = sourceProperty.GetValue(source);
            destinationProperty.SetValue(destination, value);
        }
    }
}