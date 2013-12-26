using System;
using System.IO;
using FileScanner;
using NUnit.Framework;

namespace FileScannerTest
{
    [TestFixture]
    public class SignatureGeneratorTest
    {
        private const string ResourceFolderName = "Resources";
        private const string EinsteinJpegFileName = ResourceFolderName + @"\albert-einstein.jpg";

        private static readonly DateTime EinsteinJpegCreateTime = new DateTime(2013, 12, 24, 12, 34, 56, 123, DateTimeKind.Utc);
        private static readonly DateTime EinsteinJpegModifyTime = new DateTime(2013, 12, 25, 21, 28, 48, 222, DateTimeKind.Utc);

        [TestFixtureSetUp]
        public void CreateTimeSetup()
        {
            File.SetCreationTimeUtc(EinsteinJpegFileName, EinsteinJpegCreateTime);
            File.SetLastWriteTimeUtc(EinsteinJpegFileName, EinsteinJpegModifyTime);
        }


        [Test]
        public void UpdateStatsSetsFileSize()
        {
            var sut = new FileDescriptor(EinsteinJpegFileName);
            new SignatureGenerator().UpdateStats(sut);

            Assert.That(sut.FileSize, Is.EqualTo(52439));
        }

        [Test]
        public void UpdateStatsSetsCreateTimeUtc()
        {
            var sut = new FileDescriptor(EinsteinJpegFileName);
            new SignatureGenerator().UpdateStats(sut);

            Assert.That(sut.CreateTime, Is.EqualTo(EinsteinJpegCreateTime));
        }

        [Test]
        public void UpdateStatsSetsModifyTimeUtc()
        {
            var sut = new FileDescriptor(EinsteinJpegFileName);
            new SignatureGenerator().UpdateStats(sut);

            Assert.That(sut.ModifyTime, Is.EqualTo(EinsteinJpegModifyTime));
        }

        [Test]
        public void IsFolderIsFalseForFile()
        {
            var sut = new FileDescriptor(EinsteinJpegFileName);
            new SignatureGenerator().UpdateStats(sut);

            Assert.That(sut.IsFolder, Is.False);
        }

        [Test]
        public void IsFolderIsTrueForFolder()
        {
            var sut = new FileDescriptor(ResourceFolderName);
            new SignatureGenerator().UpdateStats(sut);

            Assert.That(sut.IsFolder, Is.True);
        }

        [Test]
        public void FolderSizeIsSumOfSizeOfChildren()
        {
            var sut = new FileDescriptor(ResourceFolderName);
            new SignatureGenerator().UpdateStats(sut);

            Assert.That(sut.FileSize, Is.EqualTo(52439 + 12));
        }

        [Test]
        public void UpdateContentHashSetsSHA1ContentHashInBase64()
        {
            var sut = new FileDescriptor(EinsteinJpegFileName);
            new SignatureGenerator().UpdateContentHash(sut);

            Assert.That(sut.ContentHash, Is.EqualTo("2jKPfcACkyF04UTJEeYakT5O3+k="));
        }

    }
}
