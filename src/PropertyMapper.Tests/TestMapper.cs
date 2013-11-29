using System.Reflection;
using NUnit.Framework;
using System.Linq;

namespace PropertyMapper.Tests
{
  [TestFixture]
  public class TestMapper
  {
    [Test]
    public void can_map_simple_types_to_the_same_type()
    {
      var source = new Person {FirstName = "foo", LastName = "bar", Age = 1};
      var destination = new Person();
      
      Mapper.Map(source, destination);

      Assert.AreEqual(source.FirstName, destination.FirstName);
      Assert.AreEqual(source.LastName, destination.LastName);
      Assert.AreEqual(source.Age, destination.Age);
    }

    private class Person
    {
      public string FirstName { get; set; }
      public string LastName { get; set; }
      public int Age { get; set; }
    }
  }

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
          CopyValue(source, sourceProperty, destination, destinationProperty);
        }
      }
    }

    private static void CopyValue<TSource, TDestination>(TSource source, PropertyInfo sourceProperty, TDestination destination, PropertyInfo destinationProperty)
    {
      var value = sourceProperty.GetValue(source, null);
      destinationProperty.SetValue(destination, value, null);
    }

    private static PropertyInfo[] GetPropertiesFrom<TSource>(TSource source)
    {
      return source
        .GetType()
        .GetProperties();
    }

    private static bool IsMatch(PropertyInfo first, PropertyInfo second)
    {
      return first.PropertyType == second.PropertyType &&
             first.Name == second.Name;
    }
  }
}