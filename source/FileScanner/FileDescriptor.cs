using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace FileScanner
{
    public class FileDescriptor
    {
        public FileDescriptor(string fileName)
        {
            FileName = fileName;
        }

        public string FileName { get; private set; }

        public long FileSize { get; private set; }

        public DateTime CreateTime { get; private set; }

        public DateTime ModifyTime { get; private set; }

        public string ContentHash { get; private set; }

        public void UpdateStats()
        {
            var fileInfo = new FileInfo(FileName);
            FileSize = fileInfo.Length;
            CreateTime = fileInfo.CreationTimeUtc;
            ModifyTime = fileInfo.LastWriteTimeUtc;
        }

        public void UpdateContentHash()
        {
            using (var stream = File.OpenRead(FileName))
            {
                SHA1 sha = new SHA1CryptoServiceProvider();
                ContentHash = StringEncodeHash(sha.ComputeHash(stream));
            }
        }

        private static string StringEncodeHash(byte[] hashcode)
        {
            var hex = new StringBuilder(hashcode.Length * 2);
            foreach (var b in hashcode)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();

        }
    }
}
