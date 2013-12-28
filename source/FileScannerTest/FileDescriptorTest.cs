using System;
using System.IO;
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
        public void GetBasePathReturnsBasePath()
        {
            var sut = new FileDescriptor(Environment.CurrentDirectory, Path.GetFullPath(TestResources.EinsteinJpegPath));
            var actual = sut.GetBasePath();
            Assert.That(actual, Is.EqualTo(Environment.CurrentDirectory));
        }
   }
}
