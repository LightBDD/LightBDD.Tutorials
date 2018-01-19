using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using TutorialBuilder.Providers;

namespace TutorialBuilder
{
    public class Context
    {
        public Context(string inputDirectory, string output, string repoUrl)
        {
            InputDirectory = inputDirectory;
            RepoUrl = repoUrl;
            Name = Path.GetFileName(inputDirectory);
            OutputDirectory = output + "\\" + Name;
            Directory.CreateDirectory(OutputDirectory);
        }

        public string Name { get; }
        public string InputDirectory { get; }
        public string OutputDirectory { get; }
        public string RepoUrl { get; set; }
    }

    internal class TutorialBuilder
    {
        const string TutorialTemplateName = "tutorial.md";
        private readonly Context _context;

        public TutorialBuilder(string directory, string output, string repoUrl)
        {
            _context = new Context(directory, output,repoUrl);
        }

        public bool TryBuild()
        {
            Console.WriteLine($"Building {_context.InputDirectory}...");
            try
            {
                CompileSolution();
                RunTests();
                CompileTutorial();
                return true;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(" Failed to build tutorial: " + e.Message);
                return false;
            }
        }

        private void RunTests()
        {
            RunProcess("cmd", "/C \"test.cmd > test.log\"");
        }

        private void CompileSolution()
        {
            RunProcess("dotnet", "build -v q");
        }

        private void RunProcess(string command, string args)
        {
            var process = Process.Start(new ProcessStartInfo(command, args) { WorkingDirectory = _context.InputDirectory, UseShellExecute = false });
            process.WaitForExit();
            if (process.ExitCode != 0)
                throw new InvalidOperationException($"{command} {args} failed with code: {process.ExitCode}");
        }

        public static bool ContainsTutorial(string directory)
        {
            return File.Exists($"{directory}\\{TutorialTemplateName}");
        }

        private void CompileTutorial()
        {
            File.WriteAllText($"{_context.OutputDirectory}\\tutorial.md", new TutorialCompiler(GetTokenProcessors())
                .Compile($"{_context.InputDirectory}\\{TutorialTemplateName}"));
        }

        private IReadOnlyDictionary<string, Func<ReplacementToken, string>> GetTokenProcessors()
        {
            var typeSourceProvider = new CodeSnippetProvider(_context.InputDirectory);

            return new Dictionary<string, Func<ReplacementToken, string>>
            {
                {"link", new FileProvider(_context).ProvideLink},
                {"content", new FileProvider(_context).ProvideContent},
                {"directHtmlLink", new FileProvider(_context).ProvideDirectHtmlLink},
                {"interface", typeSourceProvider.ProvideType},
                {"class", typeSourceProvider.ProvideType},
                {"method", typeSourceProvider.ProvideMethod},
                {"text", new TextSnippetProvider(_context.InputDirectory).ProvideSnippet}
            };
        }
    }
}