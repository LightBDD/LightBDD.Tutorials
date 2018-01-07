using System;
using NUnit.Framework;
using TutorialBuilder.Providers;

namespace TutorialBuilder.Tests
{
    [TestFixture]
    public class PathLinkProviderTests
    {
        [Test]
        public void It_should_provide_link_to_directory()
        {
            Assert.That(
                ProvideLinkFor("SubFolder", "foo"),
                Is.EqualTo("[foo](SubFolder)"));
        }

        [Test]
        public void It_should_provide_link_to_file()
        {
            Assert.That(
                ProvideLinkFor("SubFolder\\SampleSourceFile.cs", "bar"),
                Is.EqualTo("[bar](SubFolder/SampleSourceFile.cs)"));
        }

        [Test]
        public void It_should_provide_link_to_file_with_default_name()
        {
            Assert.That(
                ProvideLinkFor("SubFolder\\SampleSourceFile.cs", null),
                Is.EqualTo("[SampleSourceFile.cs](SubFolder/SampleSourceFile.cs)"));
        }

        [Test]
        public void It_should_throw_if_file_or_directory_does_not_exist()
        {
            Assert.Throws<InvalidOperationException>(() => ProvideLinkFor("something.cs", null));
        }

        private static string ProvideLinkFor(string path, string named)
        {
            return new PathLinkProvider(TestHelper.TestSourceDirectory).Provide(new ReplacementToken(0, 0, "link", named, path));
        }
    }
}
