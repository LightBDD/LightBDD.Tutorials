using System;
using System.IO;

namespace TutorialBuilder.Tests
{
    public class TestHelper
    {
        public static string TestSourceDirectory { get; } = Path.GetFullPath(AppContext.BaseDirectory + "\\..\\..\\..\\Samples");
    }
}