using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TutorialBuilder.Providers
{
    static class FormattedCodeBlockSearch
    {
        public static SourceScope TryFindBlock(this SourceScope scope, Regex startPattern)
        {
            var startMatch = startPattern.Match(scope.Source, scope.Start, scope.Length);
            if (!startMatch.Success)
                return null;

            var whiteChars = CountWhiteChars(startMatch.Value);
            var endPattern = new Regex($"^\\s{{{whiteChars}}}\\}}", RegexOptions.Multiline);
            var startMatchEndIndex = startMatch.Index + startMatch.Length;
            var endMatch = endPattern.Match(scope.Source, startMatchEndIndex, scope.Start + scope.Length - startMatchEndIndex);

            if (!endMatch.Success)
                return null;
            return new SourceScope(scope.Source, startMatch.Index, endMatch.Index + endMatch.Length - startMatch.Index);
        }

        public static int CountWhiteChars(this string block, int index = 0)
        {
            return block.Skip(index).TakeWhile(Char.IsWhiteSpace).Count();
        }

        public static IEnumerable<KeyValuePair<string, SourceScope>> FindBlockInMany(this IEnumerable<KeyValuePair<string, SourceScope>> sources, Regex regex)
        {
            return sources
                .Select(x => new KeyValuePair<string, SourceScope>(x.Key, x.Value.TryFindBlock(regex)))
                .Where(x => x.Value != null);
        }
    }
}