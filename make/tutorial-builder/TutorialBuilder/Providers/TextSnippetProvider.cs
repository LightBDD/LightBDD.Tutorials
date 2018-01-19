using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TutorialBuilder.Providers
{
    public class TextSnippetProvider
    {
        private readonly Dictionary<string, SourceScope> _sources;

        public TextSnippetProvider(string directory)
        {
            _sources = Directory.EnumerateFiles(directory, "*.*", SearchOption.AllDirectories)
                .Where(path => !path.Contains("\\obj\\") && !path.Contains("\\bin\\") && !path.Contains("\\.vs\\"))
                .ToDictionary(
                    path => PathUtils.MakeRelative(path, directory),
                    path => new SourceScope(File.ReadAllText(path)));
        }

        public string ProvideSnippet(ReplacementToken token)
        {
            var parts = token.Path.Split('|');
            if (parts.Length != 3)
                throw new InvalidOperationException($"Wrong {token.Type} path format: {token.Path}");
            var file = _sources.SingleOrDefault(s => Path.GetFileName(s.Key) == parts[1]);
            var match = file.Value?.TryFindExact(new Regex(parts[2]));

            if (match == null)
                throw new InvalidOperationException($"Unable to locate {token.Type} {token.Path}");
            return ExtractSource(parts[0], file.Key, match);
        }

        private string ExtractSource(string snippetType, string sourcePath, SourceScope match)
        {
            var whiteChars = match.CountWhiteChars();
            var code = match.Substring();
            return $@"
```{snippetType}
{ShiftLeft(code, whiteChars)}
```
(see source [here]({sourcePath}#L{match.GetStartingLine()}))

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