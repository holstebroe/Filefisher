using System;
using System.IO;
using System.Linq;
using FileScanner;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace FileScannerTest
{
    [TestFixture]
    public class MemoryFileDatabaseTest
    {
        [Test]
        public void GetAllDescriptorsEmptyAfterCopnstruction()
        {
            var sut = new MemoryFileDatabase();
            Assert.That(sut.GetAllDescriptors(), Is.Empty);
        }

        [Test]
        public void GetAllDescriptorsReturnsAllDescriptors()
        {
            var sut = new MemoryFileDatabase();
            var fixture = new Fixture();
            fixture.Behaviors.Remove(fixture.Behaviors.OfType<ThrowingRecursionBehavior>().First());
            fixture.Behaviors.Add(new NullRecursionBehavior(2));
            var descriptors = fixture.CreateMany<FileDescriptor>().ToList();
            foreach (var descriptor in descriptors) sut.UpdateDescriptor(descriptor);
            Assert.That(sut.GetAllDescriptors(), Is.EquivalentTo(descriptors));
        }

        [Test]
        public void RootDescriptorIsDescriptorWithIsRootTrue()
        {
            var sut = new MemoryFileDatabase();
            var fda = new FileDescriptor("a");
            var fdb = new FileDescriptor("b") {IsRoot = true};
            var fdc = new FileDescriptor("c");
            sut.UpdateDescriptor(fda);
            sut.UpdateDescriptor(fdb);
            sut.UpdateDescriptor(fdc);

            Assert.That(sut.RootDescriptor, Is.EqualTo(fdb));
        }

        [Test]
        public void UpdateDescriptorThrowsExceptionIfNewRootIsAssigned()
        {
            var sut = new MemoryFileDatabase();
            var fda = new FileDescriptor("a") {IsRoot = true};
            var fdb = new FileDescriptor("b") {IsRoot = true};
            sut.UpdateDescriptor(fda);
            Assert.Throws<InvalidOperationException>(() => sut.UpdateDescriptor(fdb));
        }

        [Test]
        public void UpdateDescriptorDoesNotThrowExceptionIfSameRootIsAssigned()
        {
            var sut = new MemoryFileDatabase();
            var fda = new FileDescriptor("a") {IsRoot = true};
            sut.UpdateDescriptor(fda);
            sut.UpdateDescriptor(fda);
        }

        [Test]
        public void SaveLoadRoundtripRestoresAllFileDescriptors()
        {
            var database = new MemoryFileDatabase();
            var fda = new FileDescriptor("a");
            var fdb = new FileDescriptor("b") {IsRoot = true};
            var fdc = new FileDescriptor("c");
            database.UpdateDescriptor(fda);
            database.UpdateDescriptor(fdb);
            database.UpdateDescriptor(fdc);
            Assume.That(database.GetAllDescriptors().Count(), Is.EqualTo(3));
            var fileName = Path.GetTempFileName();
            try
            {
                database.Save(fileName);

                var sut = MemoryFileDatabase.Load(fileName);
                Assert.That(sut.GetAllDescriptors().Select(x => x.Path),
                    Is.EquivalentTo(database.GetAllDescriptors().Select(x => x.Path)));
                Assert.That(sut.RootDescriptor.Path, Is.EquivalentTo(database.RootDescriptor.Path));
            }
            finally
            {
                File.Delete(fileName);
            }
        }
    }
}