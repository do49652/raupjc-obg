using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace raupjc_obg.Extensions
{
    public class RegExHelperExtensions
    {
        public static string ReplaceNotMatching(string input, string pattern, string replacement, RegexOptions regexOptions = RegexOptions.None)
        {
            foreach (var c in Regex.Replace(input, pattern, "", regexOptions).ToCharArray())
                input = input.Replace(c + "", replacement);

            return input;
        }
    }
}
