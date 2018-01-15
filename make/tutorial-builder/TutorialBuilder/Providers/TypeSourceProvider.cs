using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TutorialBuilder.Providers
{
    public class TypeSourceProvider
    {
        private readonly Dictionary<string, string> _sources;

        public TypeSourceProvider(string directory)
        {
            _sources = Directory.EnumerateFileSystemEntries(directory, "*.cs", SearchOption.AllDirectories)
                .Where(path => !path.Contains("\\obj\\"))
                .ToDictionary(path => PathUtils.MakeRelative(path, directory), File.ReadAllText);
        }

        public string Provide(ReplacementToken token)
        {
            var regex = GetTypeSearchRegex(token.Type, token.Path);
            foreach (var source in _sources)
            {
                var match = new SourceScope(source.Value).TryFind(regex);
                if (match != null)
                {
                    return ExtractSource(token, source, match);
                }
            }
            throw new InvalidOperationException($"Unable to locate {token.Type} {token.Path}");
        }

        private static Regex GetTypeSearchRegex(string type, string name)
        {
            return new Regex($"(^[\\ \\t]*\\[[^\\]]+\\]\\s*\\n)*^[\\ \\t]*(\\w+[\\ \\t])*{type}[\\ \\t]+{name}\\s+", RegexOptions.Multiline);
        }

        private string ExtractSource(ReplacementToken token, KeyValuePair<string, string> source, SourceScope match)
        {
            var whiteChars = match.CountWhiteChars();
            var code = match.Substring();
            return $@"
```c#
{ShiftLeft(code, whiteChars)}
```
(see source [here]({source.Key}#L{match.GetStartingLine()}))

";
        }

        private string ShiftLeft(string code, int whiteChars)
        {
            var sb = new StringBuilder(code.Length);
            foreach (var line in code.Replace("\r", "").Split('\n'))
            {
                int pos = 0;
                while (pos < whiteChars && pos < line.Length && char.IsWhiteSpace(line[pos]))
                    ++pos;
                sb.AppendLine(line.Substring(pos));
            }

            return sb.ToString().Trim();
        }
    }

    static class FormattedCodeBlockSearch
    {
        public static SourceScope TryFind(this SourceScope scope, Regex startPattern)
        {
            var startMatch = startPattern.Match(scope.Source, scope.Start, scope.Length);
            if (!startMatch.Success)
                return null;

            var whiteChars = CountWhiteChars(startMatch.Value);
            var endPattern = new Regex($"^\\s{{{whiteChars}}}\\}}", RegexOptions.Multiline);
            var startMatchEndIndex = startMatch.Index + startMatch.Length;
            var endMatch = endPattern.Match(scope.Source, startMatchEndIndex, scope.Length - startMatchEndIndex);

            if (!endMatch.Success)
                return null;
            return new SourceScope(scope.Source, startMatch.Index, endMatch.Index + endMatch.Length - startMatch.Index);
        }

        public static int CountWhiteChars(this string block, int index = 0)
        {
            return block.Skip(index).TakeWhile(char.IsWhiteSpace).Count();
        }
    }

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
    }
}