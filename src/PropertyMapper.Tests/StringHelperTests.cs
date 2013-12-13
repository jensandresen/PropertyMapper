using NUnit.Framework;

namespace PropertyMapper.Tests
{
    [TestFixture]
    public class StringHelperTests
    {
        [TestCase("FooBar", "Foo", "Bar")]
        [TestCase("BazQux", "Baz", "Qux")]
        [TestCase("ÆblerPærer", "Æbler", "Pærer")]
        public void can_split_PascalCased_string(string input, string first, string second)
        {
            var expected = new[] {first, second};
            var result = StringHelper.SplitByPascalCasing(input);

            CollectionAssert.AreEquivalent(expected, result);
        }

        [TestCase("FooBar", "Foo", "Bar")]
        [TestCase("FooBarBaz", "Foo", "BarBaz")]
        [TestCase("FooBarBazQux", "Foo", "BarBazQux")]
        [TestCase("Foo", "Foo", null)]
        public void can_split_on_first_word(string input, string expectedWord, string expectedRemainin)
        {
            var result = StringHelper.SplitOnFirstWord(input);

            Assert.AreEqual(expectedWord, result.FirstWord);
            Assert.AreEqual(expectedRemainin, result.Remaining);
        }
    }
}