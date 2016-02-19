using System.Collections.Generic;
using System.IO;
using System.Linq;
using FileScanner;
using FileScanner.Duplicates;
using NUnit.Framework;

namespace FileScannerTest.Duplicates
{
    [TestFixture]
    public class TopDescriptorDuplicateFinderTest
    {
        [Test]
        public void EmptyDatabasesReturnsEmpty()
        {
            var dba = new MemoryFileDatabase();
            var dbb = new MemoryFileDatabase();
            var sut = new TopDescriptorDuplicateFinder(new StatDuplicateComparer());

            var actual = sut.Find(dba, dbb);
            Assert.That(actual, Is.Empty);
        }
        [Test]
        public void DifferentDatabasesReturnsEmpty()
        {
            var dba = new MemoryFileDatabase();
            dba.UpdateDescriptor(new FileDescriptor("A.txt") { StatHash = new byte[] { 1 } });
            dba.UpdateDescriptor(new FileDescriptor("B.txt") { StatHash = new byte[] { 2 } });
            var dbb = new MemoryFileDatabase();
            dbb.UpdateDescriptor(new FileDescriptor("C.txt") { StatHash = new byte[] { 3 } });
            dbb.UpdateDescriptor(new FileDescriptor("D.txt") { StatHash = new byte[] { 4 } });
            var sut = new TopDescriptorDuplicateFinder(new StatDuplicateComparer());

            var actual = sut.Find(dba, dbb);
            Assert.That(actual, Is.Empty);
        }

        [Test]
        public void OnlyFolderDuplicateReturnedForDuplicateHierarchy()
        {
            var fileSystem = @"
|A:1
 B:2
 |C1:3
  |D1:4
   E1:5
 |C2:3
  |D2:4
   E2:5
";
            var builderA = new FileDescriptorBuilder("A", 1,
                new FileDescriptorBuilder("B", 2),
                new FileDescriptorBuilder("C1", 3,
                    new FileDescriptorBuilder("D1", 4,
                        new FileDescriptorBuilder("E1", 5))));
            var builderB = new FileDescriptorBuilder("B", 10,
                new FileDescriptorBuilder("X", 11),
                new FileDescriptorBuilder("C2", 3,
                    new FileDescriptorBuilder("D2", 4,
                        new FileDescriptorBuilder("E2", 5))));
            var dba = new MemoryFileDatabase(builderA.Build());
            var dbb = new MemoryFileDatabase(builderB.Build());
            var sut = new TopDescriptorDuplicateFinder(new StatDuplicateComparer());

            var actual = sut.Find(dba, dbb).ToList();
            Assert.That(actual, Has.Count.EqualTo(1));
            Assert.That(actual.Single().Descriptors.Select(x => x.Name), Is.EqualTo(new [] {"C1", "C2"}));
        }
    }

    class FileDescriptorBuilder
    {
        public FileDescriptorBuilder(string name, byte hash, params FileDescriptorBuilder[] childBuilders)
        {
            this.name = name;
            this.hash = hash;
            foreach (var child in childBuilders)
            {
                children.Add(child);
            }
        }

        private readonly List<FileDescriptorBuilder> children = new List<FileDescriptorBuilder>();
        private readonly string name;
        private readonly byte hash;

        public FileDescriptorBuilder AddChild(string childName, byte hash)
        {
            var builder = new FileDescriptorBuilder(childName, hash);
            children.Add(builder);
            return builder;
        }

        public FileDescriptor Build()
        {
            return new FileDescriptor(name) {IsFolder = children.Any(), StatHash = new [] {hash}, Children = children.Select(x => x.Build())};
        }
    }

    //class MockFileDatabase : IFileDatabase
    //{
    //    private List<FileDescriptor> descriptors = new List<FileDescriptor>();

    //    public MockFileDatabase(string stringFileSystem)
    //    {
    //        using (var reader = new StringReader(stringFileSystem))
    //        {
    //            string line;
    //            int currentIndent = 0;
    //            var descriptor = new FileDescriptor("/") {IsRoot = true, StatHash = new byte[] {255}};
    //            var parentDescriptor = descriptor;
    //            descriptors.Add(descriptor);
    //            while ((line = reader.ReadLine()) != null)
    //            {
    //                if (string.IsNullOrWhiteSpace(line)) continue;
    //                var indent = 0;
    //                while (line[indent] == ' ') indent++;
    //                var parts = line.Substring(indent).Split(':');
    //                var fileName = parts[0];
    //                var isFolder = fileName[0] == '|';
    //                if (isFolder) fileName = fileName.Substring(1);
    //                var hash = byte.Parse(parts[1]);
    //                descriptor = new FileDescriptor(fileName) { IsFolder = isFolder, StatHash = new byte[] { hash } };

    //                if (indent > currentIndent)
    //                {
    //                    descriptor.IsFolder = true;
    //                    parentDescriptor = descriptor;
    //                }
    //            }
    //        }
    //    }
    //    public void UpdateDescriptor(FileDescriptor fileDescriptor)
    //    {
    //        throw new System.NotImplementedException();
    //    }

    //    public IEnumerable<FileDescriptor> GetAll()
    //    {
    //        throw new System.NotImplementedException();
    //    }
    //}
}
