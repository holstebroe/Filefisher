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

        private static readonly DateTime EinsteinJpegCreateTime = new DateTime(2013, 12, 24, 12, 34, 56, 123, DateTimeKind.Utc);
        private static readonly DateTime EinsteinJpegModifyTime = new DateTime(2013, 12, 25, 21, 28, 48, 222, DateTimeKind.Utc);

        [TestFixtureSetUp]
        public void CreateTimeSetup()
        {
            File.SetCreationTimeUtc(EinsteinJpegPath, EinsteinJpegCreateTime);
            File.SetLastWriteTimeUtc(EinsteinJpegPath, EinsteinJpegModifyTime);
        }

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
