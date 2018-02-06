﻿using System;
using NUnit.Framework;
using TutorialBuilder.Providers;

namespace TutorialBuilder.Tests
{
    [TestFixture]
    public class FileProviderTests
    {
        [Test]
        public void It_should_provide_link_to_directory()
        {
            Assert.That(
                ProvideLinkFor("SubFolder", "foo"),
                Is.EqualTo("[foo](https://github.com/foo/foo/blob/master/Samples/SubFolder)"));
        }

        [Test]
        public void It_should_provide_link_to_file()
        {
            Assert.That(
                ProvideLinkFor("SubFolder\\SampleSourceFile.cs", "bar"),
                Is.EqualTo("[bar](https://github.com/foo/foo/blob/master/Samples/SubFolder/SampleSourceFile.cs)"));
        }

        [Test]
        public void It_should_provide_link_to_file_with_default_name()
        {
            Assert.That(
                ProvideLinkFor("SubFolder\\SampleSourceFile.cs", null),
                Is.EqualTo("[SampleSourceFile.cs](https://github.com/foo/foo/blob/master/Samples/SubFolder/SampleSourceFile.cs)"));
        }

        [Test]
        public void It_should_throw_if_file_or_directory_does_not_exist()
        {
            Assert.Throws<InvalidOperationException>(() => ProvideLinkFor("something.cs", null));
        }

        [Test]
        public void It_should_provide_content()
        {
            Assert.That(ProvideContentFor("SubFolder\\file.txt"),
                Is.EqualTo(@"some content
other line"));
        }

        private static string ProvideContentFor(string path)
        {
            return new FileProvider(TestHelper.Context).ProvideContent(new ReplacementToken(0, 0, "content", null, path));
        }

        private static string ProvideLinkFor(string path, string named)
        {
            return new FileProvider(TestHelper.Context).ProvideLink(new ReplacementToken(0, 0, "link", named, path));
        }
    }
}