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
        public void FileDescriptorsHaveCorrectStats()
        {
            var sut = new SystemFileDescriptorProvider();
            var descriptor = new FileDescriptor(TestResources.ResourcesPath);
            var file = sut.GetFiles(descriptor).First(x => x.Path == TestResources.EinsteinJpegPath );
            Assert.That(file.Size, Is.EqualTo(52439));
            Assert.That(file.CreateTime, Is.EqualTo(TestResources.EinsteinJpegCreateTime));
            Assert.That(file.ModifyTime, Is.EqualTo(TestResources.EinsteinJpegModifyTime));
            Assert.That(file.IsFolder, Is.False);            
        }

        [Test]
        public void GetDirectoriesReturnsAllFoldersInFolder()
        {
            var sut = new SystemFileDescriptorProvider();
            var descriptor = new FileDescriptor(TestResources.ResourcesPath) { IsFolder = true};
            var folders = sut.GetDirectories(descriptor);
            Assert.That(folders.Select(x => x.Path), Is.EquivalentTo(new[] { TestResources.SubPath }));
        }

        [Test]
        public void FolderDescriptorsHaveCorrectStats()
        {
            TestResources.Initialize();
            var sut = new SystemFileDescriptorProvider();
            var descriptor = new FileDescriptor(TestResources.ResourcesPath);
            var file = sut.GetDirectories(descriptor).First(x => x.Path == TestResources.SubPath);
            Assert.That(file.Size, Is.EqualTo(0));
            Assert.That(file.CreateTime, Is.EqualTo(TestResources.SubFolderCreateTime));
            Assert.That(file.ModifyTime, Is.EqualTo(TestResources.SubFolderModifyTime));
            Assert.That(file.IsFolder, Is.True);
        }

    }
}
