using NUnit.Framework;
using TutorialBuilder.Providers;
using TutorialBuilder.Tests.Samples.SubFolder;

namespace TutorialBuilder.Tests
{
    [TestFixture]
    public class TypeSourceProviderTests
    {
        [Test]
        public void It_should_return_code_snippet_for_class()
        {
            var expected = @"
```c#
internal abstract class My_class : List<string>, IDisposable
{
    public void Dispose()
    {
    }
}
```
(see source [here](SubFolder/SampleSourceFile.cs#L7))

";
            AssertSnippet(ProvideSnippetFor("class", nameof(My_class)), expected);
        }

        [Test]
        public void It_should_return_code_snippet_for_class_with_attributes()
        {
            var expected = @"
```c#
[Description(""foo"")]
[AttributeUsage(AttributeTargets.All)]
public class MyAttribute : Attribute
{
    public int Prop { get; set; }
}
```
(see source [here](SubFolder/SampleSourceFile.cs#L14))

";
            AssertSnippet(ProvideSnippetFor("class", nameof(MyAttribute)), expected);
        }

        [Test]
        public void It_should_return_code_snippet_for_interface_with_attributes()
        {
            var expected = @"
```c#
[Description]
interface IFoo
{
    void Bar();
}
```
(see source [here](SubFolder/SampleSourceFile.cs#L21))

";
            AssertSnippet(ProvideSnippetFor("interface", nameof(IFoo)), expected);
        }

        private static string ProvideSnippetFor(string type, string path)
        {
            var provider = new TypeSourceProvider(TestHelper.TestSourceDirectory);
            return provider.Provide(new ReplacementToken(0, 0, type, null, path));
        }

        private static void AssertSnippet(string snippet, string expected)
        {
            Assert.That(snippet.Replace("\r", ""), Is.EqualTo(expected.Replace("\r", "")));
        }
    }
}
