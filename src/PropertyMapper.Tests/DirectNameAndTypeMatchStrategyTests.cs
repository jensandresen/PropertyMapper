using System;
using NUnit.Framework;

namespace PropertyMapper.Tests
{
    [TestFixture]
    public class DirectNameAndTypeMatchStrategyTests
    {
        [Test]
        public void can_match_direct_on_name()
        {
            var properties = new IProperty[]
                                 {
                                     new FakeProperty<string>("Foo"),
                                     new FakeProperty<string>("Bar"),
                                 };

            var sourceProperty = new FakeProperty<string>("Foo");
            var sut = new DirectNameAndTypeMatchStrategy(properties);

            var actual = sut.GetMatchFor(sourceProperty);

            Assert.AreEqual(sourceProperty.Name, actual.Name);
            Assert.AreEqual(sourceProperty.Type, actual.Type);
        }

        [Test]
        public void returns_null_if_no_match_was_found()
        {
            var properties = new IProperty[0];

            var sourceProperty = new FakeProperty<string>("Foo");
            var sut = new DirectNameAndTypeMatchStrategy(properties);

            var actual = sut.GetMatchFor(sourceProperty);

            Assert.IsNull(actual);
        }

    }

    public class FakeProperty<T> : IProperty
    {
        private readonly string _name;

        public FakeProperty(string name)
        {
            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }
        public Type Type
        {
            get { return typeof(T); }
        }

        public object GetValue(object instance)
        {
            throw new NotImplementedException();
        }

        public void SetValue(object instance, object value)
        {
            throw new NotImplementedException();
        }
    }
}