using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TutorialBuilder
{
    internal class TutorialCompiler
    {
        private readonly IReadOnlyDictionary<string, Func<ReplacementToken, string>> _tokenProcessors;
        private static readonly Regex TokenRegex = new Regex("%%(\\w+){((\\w+)\\|)?([^}]+)}%%", RegexOptions.Compiled);

        public TutorialCompiler(IReadOnlyDictionary<string, Func<ReplacementToken, string>> tokenProcessors)
        {
            _tokenProcessors = tokenProcessors;
        }

        public string Compile(string tutorialTemplate)
        {
            var content = File.ReadAllText(tutorialTemplate);
            var tokens = ExtractTokens(content).OrderBy(t => t.Location);

            var builder = new StringBuilder();
            var pos = 0;
            foreach (var token in tokens)
            {
                if (token.Location > pos)
                    builder.Append(content.Substring(pos, token.Location - pos));
                builder.Append(ReplaceToken(token));
                pos = token.Location + token.Length;
            }

            if (pos < content.Length)
                builder.Append(content.Substring(pos, content.Length - pos));

            return builder.ToString();
        }

        private string ReplaceToken(ReplacementToken token)
        {
            return _tokenProcessors.TryGetValue(token.Type, out var processor)
                ? processor.Invoke(token)
                : throw new InvalidOperationException(
                    $"Unable to process token '{token.Type}' - no available processor");
        }

        private IEnumerable<ReplacementToken> ExtractTokens(string content)
        {
            foreach (Match match in TokenRegex.Matches(content))
                yield return new ReplacementToken(match.Index, match.Length, match.Groups[1].Value, match.Groups[3].Value, match.Groups[4].Value);
        }
    }
}