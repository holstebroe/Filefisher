using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            var sut = new StatSignatureGenerator();
            var signature = sut.Generate(descriptor);
            var b64Signature = Convert.ToBase64String(signature);
            Assert.That(b64Signature, Is.EqualTo("bnx6xtNphTyaIB5Cz4Yw7X6nsVk="));

        }
    }
}
