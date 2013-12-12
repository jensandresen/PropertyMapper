using NUnit.Framework;
using PropertyMapper.Tests.TestDoubles;

namespace PropertyMapper.Tests
{
    [TestFixture]
    public class DirectNameAndTypeMatchStrategyTests
    {
        [Test]
        public void can_match_direct()
        {
            var properties = new IProperty[]
                                 {
                                     new FakeProperty<string>("Foo"),
                                     new FakeProperty<string>("Bar"),
                                 };

            var sourceProperty = new FakeProperty<string>("Foo");
            var sut = new DirectNameAndTypeMatchStrategy(properties);

            var actual = sut.GetMatchFor(sourceProperty);

            Assert.AreEqual(sourceProperty.Name, actual.SourceProperty.Name);
            Assert.AreEqual(sourceProperty.Type, actual.SourceProperty.Type);
        }

        [Test]
        public void must_match_both_name_and_type()
        {
            var properties = new IProperty[]
                                 {
                                     new FakeProperty<string>("Foo"),
                                     new FakeProperty<string>("Bar"),
                                 };

            var sourceProperty = new FakeProperty<int>("Foo");
            var sut = new DirectNameAndTypeMatchStrategy(properties);

            var actual = sut.GetMatchFor(sourceProperty);

            Assert.IsNull(actual);
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
}