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
    }
}