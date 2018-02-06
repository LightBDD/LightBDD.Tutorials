using NUnit.Framework;
using TutorialBuilder.Providers;
using TutorialBuilder.Tests.Samples.SubFolder;

namespace TutorialBuilder.Tests
{
    [TestFixture]
    public class CodeSnippetProviderTests
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
(source: [SubFolder/SampleSourceFile.cs](https://github.com/foo/foo/blob/master/Samples/SubFolder/SampleSourceFile.cs#L7))

";
            AssertSnippet(ProvideSnippetForType("class", nameof(My_class)), expected);
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
(source: [SubFolder/SampleSourceFile.cs](https://github.com/foo/foo/blob/master/Samples/SubFolder/SampleSourceFile.cs#L14))

";
            AssertSnippet(ProvideSnippetForType("class", nameof(MyAttribute)), expected);
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
(source: [SubFolder/SampleSourceFile.cs](https://github.com/foo/foo/blob/master/Samples/SubFolder/SampleSourceFile.cs#L21))

";
            AssertSnippet(ProvideSnippetForType("interface", nameof(IFoo)), expected);
        }

        [Test]
        public void It_should_return_code_snippet_for_method()
        {
            var expected = @"
```c#
[Description(""foo"")]
public string CallMeNow()
{
    throw new NotImplementedException();
}
```
(source: [SubFolder/SampleSourceFile.cs](https://github.com/foo/foo/blob/master/Samples/SubFolder/SampleSourceFile.cs#L56))

";
            AssertSnippet(ProvideSnippetForMethod("method", $"{nameof(MyBiggerClass)}.{nameof(MyBiggerClass.CallMeNow)}"), expected);
        }

        [Test]
        public void It_should_return_code_snippet_for_method_with_defaults()
        {
            var expected = @"
```c#
public string CallMeWithDefault(int input = 0)
{
    throw new NotImplementedException();
}
```
(source: [SubFolder/SampleSourceFile.cs](https://github.com/foo/foo/blob/master/Samples/SubFolder/SampleSourceFile.cs#L46))

";
            AssertSnippet(ProvideSnippetForMethod("method", $"{nameof(MyBiggerClass)}.{nameof(MyBiggerClass.CallMeWithDefault)}"), expected);
        }

        [Test]
        public void It_should_return_code_snippet_for_method_with_generics()
        {
            var expected = @"
```c#
public string CallMeWithGeneric<T>(T input) where T : new()
{
    throw new NotImplementedException();
}
```
(source: [SubFolder/SampleSourceFile.cs](https://github.com/foo/foo/blob/master/Samples/SubFolder/SampleSourceFile.cs#L51))

";
            AssertSnippet(ProvideSnippetForMethod("method", $"{nameof(MyBiggerClass)}.{nameof(MyBiggerClass.CallMeWithGeneric)}"), expected);
        }

        private static string ProvideSnippetForType(string type, string path)
        {
            var provider = new CodeSnippetProvider(TestHelper.Context);
            return provider.ProvideType(new ReplacementToken(0, 0, type, null, path));
        }

        private static string ProvideSnippetForMethod(string type, string path)
        {
            var provider = new CodeSnippetProvider(TestHelper.Context);
            return provider.ProvideMethod(new ReplacementToken(0, 0, type, null, path));
        }

        private static void AssertSnippet(string snippet, string expected)
        {
            Assert.That(snippet.Replace("\r", ""), Is.EqualTo(expected.Replace("\r", "")));
        }
    }
}
