using System.Collections.Generic;
using NUnit.Framework;
using PropertyMapper.Tests.TestDoubles;

namespace PropertyMapper.Tests
{
    [TestFixture]
    public class AssociationMatchStrategyTests
    {
        [Test, Ignore]
        public void can_match_an_association()
        {
            var properties = new IProperty[]
                                 {
                                     new FakeProperty<string>("BarName")
                                 };

            var sourceProperty = new FakeProperty<Foo>("Bar");
            var sut = new AssociationMatchStrategy(properties);

            sut.GetMatchFor(sourceProperty);
        }

        private class Foo
        {
            public Bar Bar { get; set; }
        }

        private class Bar
        {
            public string Name { get; set; }
        }

        private class Flattened
        {
            public string BarName { get; set; }
        }
    }

}