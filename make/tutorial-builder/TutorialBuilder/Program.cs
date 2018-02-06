using System;
using System.IO;
using System.Linq;

namespace TutorialBuilder
{
    class Program
    {
        static int Main(string[] args)
        {
            try
            {
                var baseDirectory = "tutorials";
                var tutorials = Directory.GetCurrentDirectory() + "\\" + baseDirectory;
                var output = PrepareOutputDirectory();
                foreach (var directory in Directory.EnumerateDirectories(tutorials).Where(TutorialBuilder.ContainsTutorial))
                {
                    new TutorialBuilder(new Context(baseDirectory, directory, output, "LightBDD/LightBDD.Tutorials")).TryBuild();
                }

                return 0;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                return 1;
            }
        }

        private static string PrepareOutputDirectory()
        {
            var output = Directory.GetCurrentDirectory() + "\\output";

            if (Directory.Exists(output))
                Directory.Delete(output, true);

            Directory.CreateDirectory(output);

            return output;
        }
    }
}
