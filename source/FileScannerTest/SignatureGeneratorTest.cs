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
        public void UpdateContentHashSetsSHA1ContentHashInBase64()
        {
            var sut = new FileDescriptor(TestResources.EinsteinJpegPath);
            CreateSignatureGenerator().UpdateContentHash(sut);

            Assert.That(sut.ContentHash, Is.EqualTo("2jKPfcACkyF04UTJEeYakT5O3+k="));
        }

    }
}
