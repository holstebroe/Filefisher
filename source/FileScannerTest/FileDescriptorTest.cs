using System;
using FileScanner;
using NUnit.Framework;

namespace FileScannerTest
{
    [TestFixture]
    public class FileDescriptorTest
    {
        [Test]
        public void ConstructorSetsPath()
        {
            var sut = new FileDescriptor(TestResources.EinsteinJpegPath);

            Assert.That(sut.Path, Is.EqualTo(TestResources.EinsteinJpegPath));
        }

        [Test]
        public void ConstructorExtractsNameFromProvidedPath()
        {
            var sut = new FileDescriptor(TestResources.EinsteinJpegPath);

            Assert.That(sut.Name, Is.EqualTo(TestResources.EinsteinJpegFileName));
        }

        [Test]
        public void ConstructorDoesNotSetFileSize()
        {
            var sut = new FileDescriptor(TestResources.EinsteinJpegPath);

            Assert.That(sut.Size, Is.EqualTo(0));
        }
 
        [Test]
        public void ConstructorDoesNotSetCreateTime()
        {
            var sut = new FileDescriptor(TestResources.EinsteinJpegPath);

            Assert.That(sut.CreateTime, Is.EqualTo(default(DateTime)));
        }

        [Test]
        public void ConstructorDoesNotSetModifyTime()
        {
            var sut = new FileDescriptor(TestResources.EinsteinJpegPath);

            Assert.That(sut.ModifyTime, Is.EqualTo(default(DateTime)));
        }

        [Test]
        public void ConstructorDoesNotSetContentHash()
        {
            var sut = new FileDescriptor(TestResources.EinsteinJpegPath);

            Assert.That(sut.ContentHash, Is.Null);            
        }

        [Test]
        [TestCase(@"\Bar\Boo")]
        [TestCase(@"\Bar\Boo.jpg")]
        [TestCase(@"")]
        public void GetBasePathReturnsBasePath(string localPath)
        {
            var basePath = @"C:\Test\Foo";
            Assert.That(new FileDescriptor(basePath, basePath + localPath).GetBasePath(), Is.EqualTo(basePath));
        }

        [Test]
        public void FolderPathIsRelativeToBasePath()
        {
            var basePath = @"C:\Test\Foo";
            var sut = new FileDescriptor(basePath, @"C:\Test\Foo\Bar\Boo");
            var actual = sut.Path;
            Assert.That(actual, Is.EqualTo(@"Bar\Boo"));
        }

        [Test]
        public void FilePathIsRelativeToBasePath()
        {
            var basePath = @"C:\Test\Foo";
            var sut = new FileDescriptor(basePath, @"C:\Test\Foo\Boo\Bar.jpg");
            var actual = sut.Path;
            Assert.That(actual, Is.EqualTo(@"Boo\Bar.jpg"));
        }

        [Test]
        public void RootPathIsEmpty()
        {
            var basePath = @"C:\Test\Foo";
            var sut = new FileDescriptor(basePath, basePath);
            var actual = sut.Path;
            Assert.That(actual, Is.EqualTo(@""));
        }

        [Test]
        public void IsRootTrueForRootPath()
        {
            var basePath = @"C:\Test\Foo";
            var sut = new FileDescriptor(basePath, basePath);
            var actual = sut.IsRoot;
            Assert.That(actual, Is.True);
        }


        [Test]
        public void FolderNameIsWithoutPath()
        {
            var basePath = @"C:\Test\Foo";
            var sut = new FileDescriptor(basePath, @"C:\Test\Foo\Bar\Boo");
            var actual = sut.Name;
            Assert.That(actual, Is.EqualTo(@"Boo"));
        }

        [Test]
        public void FileNameIsWithoutPath()
        {
            var basePath = @"C:\Test\Foo";
            var sut = new FileDescriptor(basePath, @"C:\Test\Foo\Boo\Bar.jpg");
            var actual = sut.Name;
            Assert.That(actual, Is.EqualTo(@"Bar.jpg"));
        }
    }
}
