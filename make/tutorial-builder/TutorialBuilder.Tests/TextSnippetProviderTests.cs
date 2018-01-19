using NUnit.Framework;
using TutorialBuilder.Providers;

namespace TutorialBuilder.Tests
{
    [TestFixture]
    public class TextSnippetProviderTests
    {
        [Test]
        public void It_should_return_code_snippet_for_regex()
        {
            var expected = @"
```c#
internal abstract class My_class : List<string>, IDisposable
```
(see source [here](SubFolder/SampleSourceFile.cs#L7))

";
            AssertSnippet(ProvideSnippetForType("text", "c#|SampleSourceFile.cs|internal abstract class My_class : List<string>, IDisposable"), expected);
        }


        private static string ProvideSnippetForType(string type, string path)
        {
            var provider = new TextSnippetProvider(TestHelper.TestSourceDirectory);
            return provider.ProvideSnippet(new ReplacementToken(0, 0, type, null, path));
        }

        private static void AssertSnippet(string snippet, string expected)
        {
            Assert.That(snippet.Replace("\r", ""), Is.EqualTo(expected.Replace("\r", "")));
        }
    }
}