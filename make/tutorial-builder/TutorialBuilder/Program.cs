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
                var tutorials = Directory.GetCurrentDirectory() + "\\tutorials";
                var output = Directory.GetCurrentDirectory() + "\\output";
                Directory.CreateDirectory(output);
                foreach (var directory in Directory.EnumerateDirectories(tutorials).Where(TutorialBuilder.ContainsTutorial))
                {
                    new TutorialBuilder(directory,output, "https://github.com/LightBDD/LightBDD.Tutorials").TryBuild();
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
