using System;
using System.IO;

namespace TutorialBuilder.Tests
{
    public class TestHelper
    {
        public static string TestSourceBaseDirectory { get; } = "Samples";
        public static string TestSourceDirectory { get; } = Path.GetFullPath(AppContext.BaseDirectory + "\\..\\..\\..\\" + TestSourceBaseDirectory);
        public static Context Context { get; } = new Context(TestSourceBaseDirectory, TestSourceDirectory, TestSourceDirectory, "foo/foo");
    }
}