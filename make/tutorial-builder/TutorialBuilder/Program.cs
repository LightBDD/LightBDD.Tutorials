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
                foreach (var directory in Directory.EnumerateDirectories(Directory.GetCurrentDirectory()).Where(TutorialBuilder.ContainsTutorial))
                {
                    new TutorialBuilder(directory).TryBuild();
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
