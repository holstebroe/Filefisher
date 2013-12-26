using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public void ScanDirectoryAddsAllFilesAndFoldersToDatabase()
        {
            var database = new MemoryFileDatabase();
            var sut = new FileCrawler(database, new SignatureGenerator());
            sut.ScanDirectory(ResourcesPath);
            var expected = new[] {EinsteinJpegPath, TextFilePath};
            var actual = database.GetAllDescriptors().Select(x => x.Path);
            Assert.That(actual, Is.EquivalentTo(expected));
        }
    }
}
