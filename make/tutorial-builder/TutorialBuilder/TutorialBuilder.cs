using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using TutorialBuilder.Providers;

namespace TutorialBuilder
{
    internal class TutorialBuilder
    {
        const string TutorialTemplateName = "tutorial.md";
        private readonly string _directory;

        public TutorialBuilder(string directory)
        {
            _directory = directory;
        }

        public bool TryBuild()
        {
            Console.WriteLine($"Building {_directory}...");
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
            var process = Process.Start(new ProcessStartInfo(command, args) { WorkingDirectory = _directory, UseShellExecute = false });
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
            File.WriteAllText($"{_directory}\\compiled_tutorial.md", new TutorialCompiler(GetTokenProcessors())
                .Compile($"{_directory}\\{TutorialTemplateName}"));
        }

        private IReadOnlyDictionary<string, Func<ReplacementToken, string>> GetTokenProcessors()
        {
            return new Dictionary<string, Func<ReplacementToken, string>>
            {
                {"link", new PathLinkProvider(_directory).Provide},
                {"interface", new TypeSourceProvider(_directory).Provide},
                {"class", new TypeSourceProvider(_directory).Provide}
            };
        }
    }
}