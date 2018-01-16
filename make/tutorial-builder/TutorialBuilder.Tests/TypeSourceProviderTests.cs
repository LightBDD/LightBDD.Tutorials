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
(see source [here](SubFolder/SampleSourceFile.cs#L14))

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
(see source [here](SubFolder/SampleSourceFile.cs#L21))

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
(see source [here](SubFolder/SampleSourceFile.cs#L56))

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
(see source [here](SubFolder/SampleSourceFile.cs#L46))

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
(see source [here](SubFolder/SampleSourceFile.cs#L51))

";
            AssertSnippet(ProvideSnippetForMethod("method", $"{nameof(MyBiggerClass)}.{nameof(MyBiggerClass.CallMeWithGeneric)}"), expected);
        }

        private static string ProvideSnippetForType(string type, string path)
        {
            var provider = new TypeSourceProvider(TestHelper.TestSourceDirectory);
            return provider.ProvideType(new ReplacementToken(0, 0, type, null, path));
        }

         private static string ProvideSnippetForMethod(string type, string path)
        {
            var provider = new TypeSourceProvider(TestHelper.TestSourceDirectory);
            return provider.ProvideMethod(new ReplacementToken(0, 0, type, null, path));
        }

        private static void AssertSnippet(string snippet, string expected)
        {
            Assert.That(snippet.Replace("\r", ""), Is.EqualTo(expected.Replace("\r", "")));
        }
    }
}
