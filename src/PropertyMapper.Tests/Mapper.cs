using System.Linq;
using System.Reflection;

namespace PropertyMapper.Tests
{
  public class Mapper
  {
    public static void Map<TSource, TDestination>(TSource source, TDestination destination)
    {
      var sourceProperties = GetPropertiesFrom(source);
      var destinationProperties = GetPropertiesFrom(destination);

      foreach (var destinationProperty in destinationProperties)
      {
        var sourceProperty = sourceProperties.SingleOrDefault(info => IsMatch(info, destinationProperty));

        if (sourceProperty != null)
        {
          CopyPropertyValue(source, sourceProperty, destination, destinationProperty);
        }
      }
    }

    private static void CopyPropertyValue(object source, PropertyInfo sourceProperty, object destination, PropertyInfo destinationProperty)
    {
      var value = sourceProperty.GetValue(source, null);
      destinationProperty.SetValue(destination, value, null);
    }

    private static PropertyInfo[] GetPropertiesFrom(object source)
    {
      return source
        .GetType()
        .GetProperties()
        .Where(p => p.GetGetMethod(false) != null)
        .Where(p => p.GetSetMethod(false) != null)
        .ToArray();
    }

    private static bool IsMatch(PropertyInfo first, PropertyInfo second)
    {
      return first.PropertyType == second.PropertyType &&
             first.Name == second.Name;
    }
  }
}