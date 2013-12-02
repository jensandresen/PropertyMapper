using NUnit.Framework;

namespace PropertyMapper.Tests
{
  [TestFixture]
  public class SimplePropertyMappingTests
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
    public void can_map_simple_properties_on_instance_of_the_same_type_with_private_setter(string expectedFirstName, int expectedAge)
    {
      var source = new PersonWithPrivateSetter("lala") { FirstName = expectedFirstName, Age = expectedAge };
      var destination = new PersonWithPrivateSetter("123");

      Mapper.Map(source, destination);

      Assert.AreEqual(expectedFirstName, destination.FirstName);
      Assert.AreEqual("123", destination.LastName);
      Assert.AreEqual(expectedAge, destination.Age);
    }
  }
}