using FileScanner;
using NUnit.Framework;

namespace FileScannerTest
{
    [TestFixture]
    public class FileDescriptorTest
    {
        private const string EinsteinJpegFileName = "albert-einstein.jpg";

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
    }
}
