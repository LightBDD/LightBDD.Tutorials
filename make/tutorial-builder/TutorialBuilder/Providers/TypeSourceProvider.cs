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
        private readonly Dictionary<string, SourceScope> _sources;

        public TypeSourceProvider(string directory)
        {
            _sources = Directory.EnumerateFileSystemEntries(directory, "*.cs", SearchOption.AllDirectories)
                .Where(path => !path.Contains("\\obj\\"))
                .ToDictionary(
                    path => PathUtils.MakeRelative(path, directory),
                    path => new SourceScope(File.ReadAllText(path)));
        }

        public string ProvideType(ReplacementToken token)
        {
            return ProvideBlock(token, _sources.FindInMany(GetTypeSearchRegex(token.Type, token.Path)));
        }

        private string ProvideBlock(ReplacementToken token, IEnumerable<KeyValuePair<string, SourceScope>> search)
        {
            var match = search.FirstOrDefault();

            if (match.Value != null)
                return ExtractSource(match.Key, match.Value);

            throw new InvalidOperationException($"Unable to locate {token.Type} {token.Path}");
        }

        public string ProvideMethod(ReplacementToken token)
        {
            var pathParts = token.Path.Split('.');
            if (pathParts.Length != 2)
                throw new InvalidOperationException($"Wrong {token.Type} path format: {token.Path}");

            var typeRegex = GetTypeSearchRegex("\\w+", pathParts[0]);
            var methodRegex = GetMethodSearchRegex(pathParts[1]);

            return ProvideBlock(token, _sources.FindInMany(typeRegex).FindInMany(methodRegex));
        }

        private static Regex GetMethodSearchRegex(string name)
        {
            return new Regex($"(^[\\ \\t]*\\[[^\\]]+\\]\\s*\\n)*^[\\ \\t]*(\\w+[\\ \\t])*[\\(\\),<>\\w\\ \\t]+[\\ \\t]+{name}\\s*(<[\\ \\t\\w,<>\\(\\)]>)?\\s*\\([\\(\\),=<>\\w\\ \\t]*\\)\\s*[\\(\\),:<>\\w\\ \\t]*\\s*\\{{", RegexOptions.Multiline);
        }

        private static Regex GetTypeSearchRegex(string type, string name)
        {
            return new Regex($"(^[\\ \\t]*\\[[^\\]]+\\]\\s*\\n)*^[\\ \\t]*(\\w+[\\ \\t])*{type}[\\ \\t]+{name}\\s+", RegexOptions.Multiline);
        }

        private string ExtractSource(string sourcePath, SourceScope match)
        {
            var whiteChars = match.CountWhiteChars();
            var code = match.Substring();
            return $@"
```c#
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