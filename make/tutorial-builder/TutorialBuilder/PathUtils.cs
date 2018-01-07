using System;

namespace TutorialBuilder
{
    public class PathUtils
    {
        public static string MakeRelative(string path, string directory)
        {
            if(!path.StartsWith(directory))
                throw new InvalidOperationException($"'{path}' does not starts with '{directory}'");
            return path.Substring(directory.Length).Replace('\\', '/').TrimStart('/');
        }
    }
}