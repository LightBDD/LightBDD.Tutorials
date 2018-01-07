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
            var regex = new Regex($"(^[\\ \\t]*\\[[^\\]]+\\]\\s*\\n)*^[\\ \\t]*(\\w+[\\ \\t])*{token.Type}[\\ \\t]+{token.Path}\\s+", RegexOptions.Multiline);
            foreach (var source in _sources)
            {
                var match = regex.Match(source.Value);
                if (match.Success)
                {
                    return ExtractSource(token, source, match);
                }
            }
            throw new InvalidOperationException($"Unable to locate {token.Type} {token.Path}");
        }

        private string ExtractSource(ReplacementToken token, KeyValuePair<string, string> source, Match startMatch)
        {
            var whiteChars = startMatch.Value.TakeWhile(char.IsWhiteSpace).Count();
            var endMatch = new Regex($"^\\s{{{whiteChars}}}\\}}", RegexOptions.Multiline).Match(source.Value, startMatch.Index + startMatch.Length);
            if (!endMatch.Success)
                throw new InvalidOperationException($"Unable to find closing brace for '{token.Type}' '{token.Path}'");

            var code = source.Value.Substring(startMatch.Index, endMatch.Index + endMatch.Length - startMatch.Index);
            return $@"
```c#
{ShiftLeft(code, whiteChars)}
```
(see source [here]({source.Key}#L{source.Value.Take(startMatch.Index).Count(x => x == '\n') + 1}))

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
}