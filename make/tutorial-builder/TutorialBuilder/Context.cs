using System.IO;

namespace TutorialBuilder
{
    public class Context
    {
        public Context(string baseDirectory, string inputDirectory, string output, string repoName)
        {
            BaseDirectory = baseDirectory;
            InputDirectory = inputDirectory;
            RepoUrl = "https://github.com/" + repoName;
            RepoRawWikiContentUrl = "https://raw.githubusercontent.com/wiki/" + repoName;
            Name = Path.GetFileName(inputDirectory);
            OutputDirectory = output + "\\" + Name;
            Directory.CreateDirectory(OutputDirectory);
        }

        public string Name { get; }
        public string BaseDirectory { get; }
        public string InputDirectory { get; }
        public string OutputDirectory { get; }
        public string RepoUrl { get; }
        public string RepoRawWikiContentUrl { get; }
    }
}