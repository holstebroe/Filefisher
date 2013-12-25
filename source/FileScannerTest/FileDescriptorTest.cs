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
    }
}
