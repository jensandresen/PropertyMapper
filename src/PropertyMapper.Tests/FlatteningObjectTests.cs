using NUnit.Framework;

namespace PropertyMapper.Tests
{
    [TestFixture]
    public class FlatteningObjectTests
    {
        [Test]
        public void can_flatten_object_graph()
        {
            var graph = new Foo {Bar = new Bar {Name = "dummy name"}};
            var flattened = new Flattened();

            Mapper.Map(graph, flattened);

            Assert.AreEqual("dummy name", flattened.BarName);
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