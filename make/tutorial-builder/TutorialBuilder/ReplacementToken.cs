using System.Linq;

namespace TutorialBuilder
{
    public class ReplacementToken
    {
        public ReplacementToken(int location, int length, string type, string name, string path)
        {
            Location = location;
            Length = length;
            Type = type;
            Name = !string.IsNullOrEmpty(name) ? name : path.Split('\\', '/').Last();
            Path = path;
        }

        public string Type { get; }
        public string Name { get; }
        public string Path { get; }
        public int Location { get; }
        public int Length { get; }
    }
}