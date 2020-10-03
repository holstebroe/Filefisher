using System.Linq;
using FileScanner;
using NUnit.Framework;

namespace FileScannerTest
{
    [TestFixture]
    public class FileCrawlerTest
    {
        [Test]
        public void ScanDirectoryReturnsRootFileDescriptor()
        {
            var database = new MemoryFileDatabase();
            var sut = new FileCrawler(database, new SystemFileDescriptorProvider(),
                new StatSignatureGenerator(new SHA1HashGenerator()), new NullProgressTracker());
            var rootDescriptor = sut.ScanDirectory(TestResources.ResourcesPath);

            Assert.That(rootDescriptor.Name, Is.EqualTo(TestResources.ResourcesPath));
            Assert.That(rootDescriptor.Children, Is.Not.Null, "Expected root descriptor to have child descriptors");
            Assert.That(rootDescriptor.Children.Select(x => x.Name),
                Is.EquivalentTo(new[] {"albert-einstein.jpg", "TextFile.txt", "Sub"}));
        }

        [Test]
        public void ScanDirectoryAddsAllFilesAndFoldersToDatabase()
        {
            var database = new MemoryFileDatabase();
            var sut = new FileCrawler(database, new SystemFileDescriptorProvider(),
                new StatSignatureGenerator(new SHA1HashGenerator()), new NullProgressTracker());
            sut.ScanDirectory(TestResources.ResourcesPath);
            var expected = TestResources.AllPaths;
            var actual = database.GetAllDescriptors().Select(x => x.Path);
            Assert.That(actual, Is.EquivalentTo(expected));
        }

        [Test]
        public void FolderSizeIsZero()
        {
            var sut = new FileCrawler(new MemoryFileDatabase(), new SystemFileDescriptorProvider(),
                new StatSignatureGenerator(new SHA1HashGenerator()), new NullProgressTracker());
            var rootDescriptor = sut.ScanDirectory(TestResources.ResourcesPath);

            Assert.That(rootDescriptor.Size, Is.EqualTo(0));
        }
    }
}