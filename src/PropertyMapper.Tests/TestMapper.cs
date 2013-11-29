using System.Reflection;
using NUnit.Framework;
using System.Linq;

namespace PropertyMapper.Tests
{
  [TestFixture]
  public class TestMapper
  {
    [TestCase("foo", "bar", 1)]
    [TestCase("baz", "qux", 2)]
    public void can_map_simple_properties_on_instances_of_the_same_type(string expectedFirstName, string expectedLastName, int expectedAge)
    {
      var source = new Person {FirstName = expectedFirstName, LastName = expectedLastName, Age = expectedAge};
      var destination = new Person();
      
      Mapper.Map(source, destination);

      Assert.AreEqual(expectedFirstName, destination.FirstName);
      Assert.AreEqual(expectedLastName, destination.LastName);
      Assert.AreEqual(expectedAge, destination.Age);
    }

    [TestCase("foo", 1)]
//    [TestCase("baz", "qux", 2)]
    public void can_map_simple_properties_on_instance_of_the_same_type_with_private_setter(string expectedFirstName, int expectedAge)
    {
      var source = new PersonWithPrivateSetter("lala") { FirstName = expectedFirstName, Age = expectedAge };
      var destination = new PersonWithPrivateSetter("123");

      Mapper.Map(source, destination);

      Assert.AreEqual(expectedFirstName, destination.FirstName);
      Assert.AreEqual("123", destination.LastName);
      Assert.AreEqual(expectedAge, destination.Age);
    }

    private class Person
    {
      public string FirstName { get; set; }
      public string LastName { get; set; }
      public int Age { get; set; }
    }

    private class PersonWithPrivateSetter
    {
      public PersonWithPrivateSetter(string lastName)
      {
        LastName = lastName;
      }

      public string FirstName { get; set; }
      public string LastName { get; private set; }
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