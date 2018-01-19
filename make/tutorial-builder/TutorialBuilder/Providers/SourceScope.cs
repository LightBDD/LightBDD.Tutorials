using System.Linq;
using System.Text.RegularExpressions;

namespace TutorialBuilder.Providers
{
    class SourceScope
    {
        public string Source { get; }
        public int Start { get; }
        public int Length { get; }

        public SourceScope(string source)
            : this(source, 0, source.Length)
        {
        }

        public SourceScope(string source, int start, int length)
        {
            Source = source;
            Start = start;
            Length = length;
        }

        public int CountWhiteChars()
        {
            return Source.CountWhiteChars(Start);
        }

        public string Substring()
        {
            return Source.Substring(Start, Length);
        }

        public int GetStartingLine()
        {
            return Source.Take(Start).Count(x => x == '\n') + 1;
        }

        public SourceScope TryFindExact(Regex regex)
        {
            var match = regex.Match(Source, Start, Length);
            if (!match.Success)
                return null;
            return new SourceScope(Source, match.Index, match.Length);
        }
    }
}