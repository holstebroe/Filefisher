using FileScanner;
using FileScanner.Duplicates;
using NUnit.Framework;

namespace FileScannerTest.Duplicates
{
    [TestFixture]
    public class StatDuplicateComparerTest
    {
        [Test]
        public void SameFileIsNotDuplicate()
        {
            var sut = new StatDuplicateComparer();
            var descriptor = new FileDescriptor(TestResources.TextFilePath) {StatHash = new byte[] {1, 2, 3}};
            var actual = sut.Equals(descriptor, descriptor);
            Assert.That(actual, Is.False);
        }

        [Test]
        public void SameStatHashAreDuplicates()
        {
            var sut = new StatDuplicateComparer();
            var descriptorA = new FileDescriptor(TestResources.TextFilePath) {StatHash = new byte[] {1, 1, 1}};
            var descriptorB = new FileDescriptor(TestResources.SubTextFilePath) {StatHash = new byte[] {1, 1, 1}};
            var actual = sut.Equals(descriptorA, descriptorB);
            Assert.That(actual, Is.True);
        }

        [Test]
        public void DifferentStatHashAreNotDuplicates()
        {
            var sut = new StatDuplicateComparer();
            var descriptorA = new FileDescriptor(TestResources.TextFilePath) {StatHash = new byte[] {1, 1, 1}};
            var descriptorB = new FileDescriptor(TestResources.SubTextFilePath) {StatHash = new byte[] {2, 2, 2}};
            var actual = sut.Equals(descriptorA, descriptorB);
            Assert.That(actual, Is.False);
        }
    }
}