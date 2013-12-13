using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace PropertyMapper
{
    public static class StringHelper
    {
        public static IEnumerable<string> SplitByPascalCasing(string text)
        {
            return Regex.Split(text, "(?<!^)(?=[A-Z])");
        }

        public static SplitResult SplitOnFirstWord(string text)
        {
            var temp = SplitByPascalCasing(text);
            var firstWord = temp.FirstOrDefault();
            var remainingWords = temp.Skip(1).ToArray();

            return new SplitResult
            {
                FirstWord = firstWord,
                Remaining = remainingWords.Length > 0 ? string.Join("", remainingWords) : null,
            };
        }
    }

    public class SplitResult
    {
        public string FirstWord { get; set; }
        public string Remaining { get; set; }
    }
}