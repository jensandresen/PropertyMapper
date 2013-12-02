using NUnit.Framework;

namespace PropertyMapper.Tests
{
  [TestFixture]
  public class IgnoringPropertiesTests
  {
    [Test]
    public void can_ignore_single_property()
    {
      var source = new Person {FirstName = "foo", LastName = "bar", Age = 1};
      var destination = new Person();
      
      Mapper.Map(source, destination, cfg =>
      {
        cfg.Ignore(x => x.LastName);
      });

      Assert.AreEqual(source.FirstName, destination.FirstName);
      Assert.AreEqual(source.Age, destination.Age);
      Assert.IsNull(destination.LastName);
    }

    [Test]
    public void can_ignore_multiple_properties()
    {
      var source = new Person {FirstName = "foo", LastName = "bar", Age = 1};
      var destination = new Person();
      
      Mapper.Map(source, destination, cfg =>
      {
        cfg.Ignore(x => x.FirstName);
        cfg.Ignore(x => x.LastName);
      });

      Assert.AreEqual(source.Age, destination.Age);
      Assert.IsNull(destination.FirstName);
      Assert.IsNull(destination.LastName);
    }

  }
}