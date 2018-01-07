using System;
using System.IO;

namespace TutorialBuilder.Providers
{
    public class PathLinkProvider
    {
        private readonly string _directory;

        public PathLinkProvider(string directory)
        {
            _directory = directory;
        }

        public string Provide(ReplacementToken token)
        {
            var path = $"{_directory}\\{token.Path}";
            if (!Directory.Exists(path) && !File.Exists(path))
                throw new InvalidOperationException($"File does not exists: '{path}'");

            return $"[{token.Name}]({PathUtils.MakeRelative(path, _directory)})";
        }
    }
}