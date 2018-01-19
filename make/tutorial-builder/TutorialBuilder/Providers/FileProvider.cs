using System;
using System.IO;

namespace TutorialBuilder.Providers
{
    public class FileProvider
    {
        private readonly Context _context;

        public FileProvider(Context context)
        {
            _context = context;
        }

        public string ProvideLink(ReplacementToken token)
        {
            var path = GetPath(token, true);
            return $"[{token.Name}]({PathUtils.MakeRelative(path, _context.InputDirectory)})";
        }

        public string ProvideContent(ReplacementToken token)
        {
            return File.ReadAllText(GetPath(token, false));
        }

        private string GetPath(ReplacementToken token, bool includeDirectories)
        {
            var path = $"{_context.InputDirectory}\\{token.Path}";
            if (File.Exists(path) || includeDirectories && Directory.Exists(path))
                return path;
            throw new InvalidOperationException($"File does not exists: '{path}'");
        }

        public string ProvideDirectHtmlLink(ReplacementToken token)
        {
            var path = GetPath(token, false);
            var fileName = Path.GetFileName(path);
            File.Copy(path,_context.OutputDirectory+"\\"+fileName);
            return $"[{fileName}](http://htmlpreview.github.io/?{_context.RepoUrl}/wiki/{_context.Name}/{fileName})";
        }
    }
}