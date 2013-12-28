using System;
using FileScanner;
using NUnit.Framework;

namespace FileScannerTest
{
    [TestFixture]
    public class ContentSignatureGeneratorTest
    {
        [Test]
        public void GeneratesFileSignatureFromNameModifyAndSize()
        {
            var descriptor = new FileDescriptor("myfile.txt");
            descriptor.ModifyTime = new DateTime(2013, 12, 24, 0, 0, 0, DateTimeKind.Utc);
            descriptor.Size = 1024;

            var sut = new ContentSignatureGenerator(new SHA1HashGenerator());
            sut.UpdateFileSignature(descriptor);
            var b64Signature = Convert.ToBase64String(descriptor.ContentHash);
            Assert.That(b64Signature, Is.EqualTo("bnx6xtNphTyaIB5Cz4Yw7X6nsVk="));
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
