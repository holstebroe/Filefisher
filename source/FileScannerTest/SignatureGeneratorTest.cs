﻿using System;
using System.IO;
using FileScanner;
using NUnit.Framework;

namespace FileScannerTest
{
    [TestFixture]
    public class SignatureGeneratorTest
    {
        private const string ResourcesPath = "Resources";
        private const string EinsteinJpegPath = ResourcesPath + @"\albert-einstein.jpg";

        private static readonly DateTime EinsteinJpegCreateTime = new DateTime(2013, 12, 24, 12, 34, 56, 123, DateTimeKind.Utc);
        private static readonly DateTime EinsteinJpegModifyTime = new DateTime(2013, 12, 25, 21, 28, 48, 222, DateTimeKind.Utc);

        [TestFixtureSetUp]
        public void CreateTimeSetup()
        {
            File.SetCreationTimeUtc(EinsteinJpegPath, EinsteinJpegCreateTime);
            File.SetLastWriteTimeUtc(EinsteinJpegPath, EinsteinJpegModifyTime);
        }

        private SignatureGenerator CreateSignatureGenerator()
        {
            return new SignatureGenerator(new SHA1HashGenerator());
        }

        [Test]
        public void UpdateStatsSetsFileSize()
        {
            var sut = new FileDescriptor(EinsteinJpegPath);
            CreateSignatureGenerator().UpdateStats(sut);

            Assert.That(sut.Size, Is.EqualTo(52439));
        }

        [Test]
        public void UpdateStatsSetsCreateTimeUtc()
        {
            var sut = new FileDescriptor(EinsteinJpegPath);
            CreateSignatureGenerator().UpdateStats(sut);

            Assert.That(sut.CreateTime, Is.EqualTo(EinsteinJpegCreateTime));
        }

        [Test]
        public void UpdateStatsSetsModifyTimeUtc()
        {
            var sut = new FileDescriptor(EinsteinJpegPath);
            CreateSignatureGenerator().UpdateStats(sut);

            Assert.That(sut.ModifyTime, Is.EqualTo(EinsteinJpegModifyTime));
        }

        [Test]
        public void IsFolderIsFalseForFile()
        {
            var sut = new FileDescriptor(EinsteinJpegPath);
            CreateSignatureGenerator().UpdateStats(sut);

            Assert.That(sut.IsFolder, Is.False);
        }

        [Test]
        public void IsFolderIsTrueForFolder()
        {
            var sut = new FileDescriptor(ResourcesPath);
            CreateSignatureGenerator().UpdateStats(sut);

            Assert.That(sut.IsFolder, Is.True);
        }

        [Test]
        public void UpdateContentHashSetsSHA1ContentHashInBase64()
        {
            var sut = new FileDescriptor(EinsteinJpegPath);
            CreateSignatureGenerator().UpdateContentHash(sut);

            Assert.That(sut.ContentHash, Is.EqualTo("2jKPfcACkyF04UTJEeYakT5O3+k="));
        }

    }
}
