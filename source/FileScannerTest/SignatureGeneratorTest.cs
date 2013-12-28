using System;
using System.IO;
using FileScanner;
using NUnit.Framework;

namespace FileScannerTest
{
    [TestFixture]
    public class SignatureGeneratorTest
    {
        [TestFixtureSetUp]
        public void CreateTimeSetup()
        {
            File.SetCreationTimeUtc(TestResources.EinsteinJpegPath, TestResources.EinsteinJpegCreateTime);
            File.SetLastWriteTimeUtc(TestResources.EinsteinJpegPath, TestResources.EinsteinJpegModifyTime);
        }

        private SignatureGenerator CreateSignatureGenerator()
        {
            return new SignatureGenerator(new SHA1HashGenerator());
        }

        [Test]
        public void UpdateStatsSetsFileSize()
        {
            var sut = new FileDescriptor(TestResources.EinsteinJpegPath);
            CreateSignatureGenerator().UpdateStats(sut);

            Assert.That(sut.Size, Is.EqualTo(52439));
        }

        [Test]
        public void UpdateStatsSetsCreateTimeUtc()
        {
            var sut = new FileDescriptor(TestResources.EinsteinJpegPath);
            CreateSignatureGenerator().UpdateStats(sut);

            Assert.That(sut.CreateTime, Is.EqualTo(TestResources.EinsteinJpegCreateTime));
        }

        [Test]
        public void UpdateStatsSetsModifyTimeUtc()
        {
            var sut = new FileDescriptor(TestResources.EinsteinJpegPath);
            CreateSignatureGenerator().UpdateStats(sut);

            Assert.That(sut.ModifyTime, Is.EqualTo(TestResources.EinsteinJpegModifyTime));
        }

        [Test]
        public void IsFolderIsFalseForFile()
        {
            var sut = new FileDescriptor(TestResources.EinsteinJpegPath);
            CreateSignatureGenerator().UpdateStats(sut);

            Assert.That(sut.IsFolder, Is.False);
        }

        [Test]
        public void IsFolderIsTrueForFolder()
        {
            var sut = new FileDescriptor(TestResources.ResourcesPath);
            CreateSignatureGenerator().UpdateStats(sut);

            Assert.That(sut.IsFolder, Is.True);
        }

        [Test]
        public void UpdateContentHashSetsSHA1ContentHashInBase64()
        {
            var sut = new FileDescriptor(TestResources.EinsteinJpegPath);
            CreateSignatureGenerator().UpdateContentHash(sut);

            Assert.That(sut.ContentHash, Is.EqualTo("2jKPfcACkyF04UTJEeYakT5O3+k="));
        }

    }
}
