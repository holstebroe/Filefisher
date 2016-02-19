using System.Linq;
using FileScanner;
using FileScanner.Duplicates;
using NUnit.Framework;

namespace FileScannerTest.Duplicates
{
    [TestFixture]
    public class FileDuplicateFinderTest
    {
        [Test]
        public void ComparingEmptyDatabasesReturnsEmpty()
        {
            var dba = new MemoryFileDatabase();
            var dbb = new MemoryFileDatabase();
            var sut = new FileDuplicateFinder(new StatDuplicateComparer());

            var actual = sut.Find(dba, dbb);
            Assert.That(actual, Is.Empty);
        }

        [Test]
        public void DifferentDatabasesReturnsEmpty()
        {
            var dba = new MemoryFileDatabase();
            dba.UpdateDescriptor(new FileDescriptor("A.txt") {StatHash = new byte[] { 1 } });
            dba.UpdateDescriptor(new FileDescriptor("B.txt") { StatHash = new byte[] { 2 } });
            var dbb = new MemoryFileDatabase();
            dbb.UpdateDescriptor(new FileDescriptor("C.txt") { StatHash = new byte[] { 3 } });
            dbb.UpdateDescriptor(new FileDescriptor("D.txt") { StatHash = new byte[] { 4 } });
            var sut = new FileDuplicateFinder(new StatDuplicateComparer());

            var actual = sut.Find(dba, dbb);
            Assert.That(actual, Is.Empty);
        }

        [Test]
        public void ReturnsAllDuplicates()
        {
            var dba = new MemoryFileDatabase();
//            dba.UpdateDescriptor(new FileDescriptor("/") { IsRoot = true});
            dba.UpdateDescriptor(new FileDescriptor("A.txt") {StatHash = new byte[] { 1 } });
            dba.UpdateDescriptor(new FileDescriptor("B.txt") { StatHash = new byte[] { 2 } });
            var dbb = new MemoryFileDatabase();
//            dbb.UpdateDescriptor(new FileDescriptor("/") { IsRoot = true });
            dbb.UpdateDescriptor(new FileDescriptor("C.txt") { StatHash = new byte[] { 1 } });
            dbb.UpdateDescriptor(new FileDescriptor("D.txt") { StatHash = new byte[] { 4 } });
            dbb.UpdateDescriptor(new FileDescriptor("E.txt") { StatHash = new byte[] { 2 } });
            var sut = new FileDuplicateFinder(new StatDuplicateComparer());

            var actual = sut.Find(dba, dbb).ToList();
            Assert.That(actual, Has.Count.EqualTo(2));
            Assert.That(actual.First().Descriptors.Select(x => x.Name), Is.EqualTo(new [] { "A.txt", "C.txt"}));
            Assert.That(actual.Last().Descriptors.Select(x => x.Name), Is.EqualTo(new [] { "B.txt", "E.txt"}));
        }
    }
}
