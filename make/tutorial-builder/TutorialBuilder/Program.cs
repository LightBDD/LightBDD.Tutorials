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
                var output = Directory.GetCurrentDirectory() + "\\output";
                Directory.CreateDirectory(output);
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
    }
}
