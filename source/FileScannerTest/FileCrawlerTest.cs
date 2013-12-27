using System.Linq;
using FileScanner;
using NUnit.Framework;

namespace FileScannerTest
{
    [TestFixture]
    public class FileCrawlerTest
    {
        private const string ResourcesPath = "Resources";
        private const string EinsteinJpegPath = ResourcesPath + @"\albert-einstein.jpg";
        private const string TextFilePath = ResourcesPath + @"\TextFile.txt";


        [Test]
        public void ScanDirectoryReturnsRootFileDescriptor()
        {
            var database = new MemoryFileDatabase();
            var sut = new FileCrawler(database, new SignatureGenerator(new SHA1HashGenerator()));
            var rootDescriptor = sut.ScanDirectory(ResourcesPath);

            Assert.That(rootDescriptor.Name, Is.EqualTo(ResourcesPath));
            Assert.That(rootDescriptor.Children, Is.Not.Null, "Expected root descriptor to have child descriptors");
            Assert.That(rootDescriptor.Children.Select(x => x.Name), Is.EquivalentTo(new [] {"albert-einstein.jpg", "TextFile.txt"}));
        }

        [Test]
        public void ScanDirectoryAddsAllFilesAndFoldersToDatabase()
        {
            var database = new MemoryFileDatabase();
            var sut = new FileCrawler(database, new SignatureGenerator(new SHA1HashGenerator()));
            sut.ScanDirectory(ResourcesPath);
            var expected = new[] {EinsteinJpegPath, TextFilePath, ResourcesPath};
            var actual = database.GetAllDescriptors().Select(x => x.Path);
            Assert.That(actual, Is.EquivalentTo(expected));
        }

        [Test]
        public void FolderSizeIsSumOfSizeOfChildren()
        {
            var sut = new FileCrawler(new MemoryFileDatabase(), new SignatureGenerator(new SHA1HashGenerator()));
            var rootDescriptor = sut.ScanDirectory(ResourcesPath);

            Assert.That(rootDescriptor.Size, Is.EqualTo(52439 + 12));
        }

    }
}
