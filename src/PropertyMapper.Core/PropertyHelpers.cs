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

        public static PropertyInfo[] GetAvailablePropertiesFrom(Type targetedType)
        {
            return targetedType
                .GetProperties()
                .Where(p => p.GetGetMethod(false) != null)
                .Where(p => p.GetSetMethod(false) != null)
                .ToArray();
        }
    }
}