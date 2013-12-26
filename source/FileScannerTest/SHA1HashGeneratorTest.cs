using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileScanner;
using NUnit.Framework;

namespace FileScannerTest
{
    [TestFixture]
    public class SHA1HashGeneratorTest
    {
        [Test]
        public void GeneratesCorrectSHA1HashFromEmptyBytes()
        {
            var sut = new SHA1HashGenerator();
            var b64Signature = Convert.ToBase64String(sut.Generate(new byte[0]));
            Assert.That(b64Signature, Is.EqualTo("2jmj7l5rSw0yVb/vlWAYkK/YBwk="));            
        }
        
        [Test]
        public void GeneratesCorrectSHA1HashFromMultiBytes()
        {
            var sut = new SHA1HashGenerator();
            var b64Signature = Convert.ToBase64String(sut.Generate(new byte[] { 1,2,3,4,5,6,7,8,9,10}));
            Assert.That(b64Signature, Is.EqualTo("xTkeMIryW0LVk01qIBo06JjSVcY="));            
        }
    }
}
