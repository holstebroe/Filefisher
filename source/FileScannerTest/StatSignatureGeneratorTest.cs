using System;
using FileScanner;
using NUnit.Framework;

namespace FileScannerTest
{
    [TestFixture]
    public class StatSignatureGeneratorTest
    {
        [Test]
        public void GeneratesFileSignatureFromNameModifyAndSize()
        {
            var descriptor = new FileDescriptor("myfile.txt");
            descriptor.ModifyTime = new DateTime(2013, 12, 24, 0, 0, 0, DateTimeKind.Utc);
            descriptor.Size = 1024;

            var sut = new StatSignatureGenerator(new SHA1HashGenerator());
            sut.Generate(descriptor);
            var b64Signature = Convert.ToBase64String(descriptor.StatHash);
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
                            new FileDescriptor("MyFileA.txt") {StatHash = new byte[] {1, 2, 3}},
                            new FileDescriptor("MyFileB.txt") {StatHash = new byte[] {10, 20, 30}}
                        }
                };

            var sut = new StatSignatureGenerator(new SHA1HashGenerator());
            sut.Generate(descriptor);
            var b64Signature = Convert.ToBase64String(descriptor.StatHash);
            Assert.That(b64Signature, Is.EqualTo("6UeAJmWx6C9gYXXVROOHSNcoN8U="));
        }
    }
}
