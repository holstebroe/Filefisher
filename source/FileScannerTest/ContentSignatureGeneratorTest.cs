using System;
using FileScanner;
using NUnit.Framework;

namespace FileScannerTest
{
    [TestFixture]
    public class ContentSignatureGeneratorTest
    {
        [Test]
        public void GeneratesFileSignatureFromFileContent()
        {
            var descriptor = new FileDescriptor(TestResources.TextFilePath);
            // Set modify time and size to random values to prove that these are ignored
            descriptor.ModifyTime = DateTime.Now;
            descriptor.Size = descriptor.ModifyTime.Millisecond;

            var sut = new ContentSignatureGenerator(new SHA1HashGenerator());
            sut.UpdateFileSignature(descriptor);
            var b64Signature = Convert.ToBase64String(descriptor.ContentHash);
            Assert.That(b64Signature, Is.EqualTo("OkwbOzKOydWv3BCGQt/udXjMlx8="));
        }

        [Test]
        public void GeneratesFolderSignatureFromChildren()
        {
            var descriptor = new FileDescriptor("Folder")
                {
                    IsFolder = true,
                    Children = new[]
                        {
                            new FileDescriptor("MyFileA.txt") {ContentHash = new byte[] {1, 2, 3}},
                            new FileDescriptor("MyFileB.txt") {ContentHash = new byte[] {10, 20, 30}}
                        }
                };

            var sut = new ContentSignatureGenerator(new SHA1HashGenerator());
            sut.UpdateFolderSignature(descriptor);
            var b64Signature = Convert.ToBase64String(descriptor.ContentHash);
            Assert.That(b64Signature, Is.EqualTo("6UeAJmWx6C9gYXXVROOHSNcoN8U="));
        }
    }
}
