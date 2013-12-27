using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            foreach (var descriptor in descriptors)
            {
                sut.UpdateDescriptor(descriptor);               
            }
            Assert.That(sut.GetAllDescriptors(), Is.EquivalentTo(descriptors));
        }
    }
}
