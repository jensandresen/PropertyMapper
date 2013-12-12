using System;
using System.Linq;

namespace PropertyMapper
{
    public static class PropertyHelpers
    {
        public static bool IsMatch(IProperty first, IProperty second)
        {
            return IsMatch(first.Type, first.Name, second.Type, second.Name);
        }

        public static bool IsMatch(Type sourceType, string sourceName, Type destinationType, string destinationName)
        {
            return sourceType == destinationType &&
                   sourceName == destinationName;
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
    }
}