using System;
using System.IO;
using FileScanner;
using NUnit.Framework;

namespace FileScannerTest
{
    [TestFixture]
    public class FileDescriptorTest
    {
        private const string ResourcesPath = "Resources";
        private const string EinsteinJpegPath = ResourcesPath + @"\albert-einstein.jpg";

        [Test]
        public void ConstructorSetsPath()
        {
            var sut = new FileDescriptor(EinsteinJpegPath);

            Assert.That(sut.Path, Is.EqualTo(EinsteinJpegPath));
        }

        [Test]
        public void ConstructorDoesNotSetFileSize()
        {
            var sut = new FileDescriptor(EinsteinJpegPath);

            Assert.That(sut.Size, Is.EqualTo(0));
        }
 
        [Test]
        public void ConstructorDoesNotSetCreateTime()
        {
            var sut = new FileDescriptor(EinsteinJpegPath);

            Assert.That(sut.CreateTime, Is.EqualTo(default(DateTime)));
        }

        [Test]
        public void ConstructorDoesNotSetModifyTime()
        {
            var sut = new FileDescriptor(EinsteinJpegPath);

            Assert.That(sut.ModifyTime, Is.EqualTo(default(DateTime)));
        }

        [Test]
        public void ConstructorDoesNotSetContentHash()
        {
            var sut = new FileDescriptor(EinsteinJpegPath);

            Assert.That(sut.ContentHash, Is.Null);            
        }
   }
}
