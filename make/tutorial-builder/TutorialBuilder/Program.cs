using System.IO;
using System.Linq;

namespace TutorialBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach (var directory in Directory.EnumerateDirectories(Directory.GetCurrentDirectory()).Where(TutorialBuilder.ContainsTutorial))
            {
                new TutorialBuilder(directory).TryBuild();
            }
        }
    }
}
