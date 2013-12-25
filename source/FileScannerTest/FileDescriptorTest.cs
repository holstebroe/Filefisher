using System;
using System.IO;
using FileScanner;
using NUnit.Framework;

namespace FileScannerTest
{
    [TestFixture]
    public class FileDescriptorTest
    {
        private const string EinsteinJpegFileName = "albert-einstein.jpg";

        private static readonly DateTime EinsteinJpegCreateTime = new DateTime(2013, 12, 24, 12, 34, 56, 123, DateTimeKind.Utc);
        private static readonly DateTime EinsteinJpegModifyTime = new DateTime(2013, 12, 25, 21, 28, 48, 222, DateTimeKind.Utc);

        [TestFixtureSetUp]
        public void CreateTimeSetup()
        {
            File.SetCreationTimeUtc(EinsteinJpegFileName, EinsteinJpegCreateTime);
            File.SetLastWriteTimeUtc(EinsteinJpegFileName, EinsteinJpegModifyTime);
        }

        [Test]
        public void ConstructorSetsFileName()
        {
            var sut = new FileDescriptor(EinsteinJpegFileName);

            Assert.That(sut.FileName, Is.EqualTo(EinsteinJpegFileName));
        }

        [Test]
        public void ConstructorDoesNotSetFileSize()
        {
            var sut = new FileDescriptor(EinsteinJpegFileName);

            Assert.That(sut.FileSize, Is.EqualTo(0));
        }
 
        [Test]
        public void UpdateStatsSetsFileSize()
        {
            var sut = new FileDescriptor(EinsteinJpegFileName);
            sut.UpdateStats();

            Assert.That(sut.FileSize, Is.EqualTo(52439));
        }
 
        [Test]
        public void ConstructorDoesNotSetCreateTime()
        {
            var sut = new FileDescriptor(EinsteinJpegFileName);

            Assert.That(sut.CreateTime, Is.EqualTo(default(DateTime)));
        }

        [Test]
        public void UpdateStatsSetsCreateTimeUtc()
        {
            var sut = new FileDescriptor(EinsteinJpegFileName);
            sut.UpdateStats();

            Assert.That(sut.CreateTime, Is.EqualTo(EinsteinJpegCreateTime));
        }

        [Test]
        public void ConstructorDoesNotSetModifyTime()
        {
            var sut = new FileDescriptor(EinsteinJpegFileName);

            Assert.That(sut.ModifyTime, Is.EqualTo(default(DateTime)));
        }

        [Test]
        public void UpdateStatsSetsModifyTimeUtc()
        {
            var sut = new FileDescriptor(EinsteinJpegFileName);
            sut.UpdateStats();

            Assert.That(sut.ModifyTime, Is.EqualTo(EinsteinJpegModifyTime));
        }

        [Test]
        public void ConstructorDoesNotSetContentHash()
        {
            var sut = new FileDescriptor(EinsteinJpegFileName);

            Assert.That(sut.ContentHash, Is.Null);            
        }

        [Test]
        public void UpdateContentHashSetsContentHash()
        {
            var sut = new FileDescriptor(EinsteinJpegFileName);
            sut.UpdateContentHash();

            Assert.That(sut.ContentHash, Is.EqualTo("da328f7dc002932174e144c911e61a913e4edfe9"));
        }


   }
}
