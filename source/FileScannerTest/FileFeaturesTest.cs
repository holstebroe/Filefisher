using FileScanner;
using NUnit.Framework;

namespace FileScannerTest
{
    [TestFixture]
    public class FileFeaturesTest
    {
        private const string EinsteinJpegFileName = "albert-einstein.jpg";

        [Test]
        public void ConstructorSetsFileName()
        {
            var sut = new FileFeatures(EinsteinJpegFileName);

            Assert.That(sut.FileName, Is.EqualTo(EinsteinJpegFileName));
        }
    }
}
