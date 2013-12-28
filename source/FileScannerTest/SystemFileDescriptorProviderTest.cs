using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileScanner;
using NUnit.Framework;

namespace FileScannerTest
{
    [TestFixture]
    public class SystemFileDescriptorProviderTest
    {
        [Test]
        public void GetFilesReturnsAllFilesInFolder()
        {
            var sut = new SystemFileDescriptorProvider();
            var descriptor = new FileDescriptor(TestResources.ResourcesPath);
            var files = sut.GetFiles(descriptor);
            Assert.That(files.Select(x => x.Path), Is.EquivalentTo(new [] { TestResources.EinsteinJpegPath, TestResources.TextFilePath}));
        }

        [Test]
        public void GetDirectoriesReturnsAllFoldersInFolder()
        {
            var sut = new SystemFileDescriptorProvider();
            var descriptor = new FileDescriptor(TestResources.ResourcesPath) { IsFolder = true};
            var folders = sut.GetDirectories(descriptor);
            Assert.That(folders.Select(x => x.Path), Is.EquivalentTo(new[] { TestResources.SubPath }));
        }
    }
}
