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
        private readonly Context _context;

        public TutorialBuilder(Context context)
        {
            _context = context;
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
            File.WriteAllText($"{_context.OutputDirectory}\\{_context.Name}.md", new TutorialCompiler(GetTokenProcessors())
                .Compile($"{_context.InputDirectory}\\{TutorialTemplateName}"));
        }

        private IReadOnlyDictionary<string, Func<ReplacementToken, string>> GetTokenProcessors()
        {
            var typeSourceProvider = new CodeSnippetProvider(_context);

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