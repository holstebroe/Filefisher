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

        private static readonly DateTime EinsteinJpegCreateTime = new DateTime(2013, 12, 24, 12, 34, 56,
                                                                               DateTimeKind.Utc);

        [TestFixtureSetUp]
        public void CreateTimeSetup()
        {
            File.SetCreationTimeUtc(EinsteinJpegFileName, EinsteinJpegCreateTime);
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

   }
}
