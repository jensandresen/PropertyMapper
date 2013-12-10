using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PropertyMapper
{
    public static class StringHelper
    {
        public static IEnumerable<string> SplitByPascalCasing(string text)
        {
            return Regex.Split(text, "(?<!^)(?=[A-Z])");
        }
    }
}