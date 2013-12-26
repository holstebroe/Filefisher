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

        public bool IsFolder { get; private set; }

        public void UpdateStats()
        {
            var attributes = File.GetAttributes(FileName);
            IsFolder = attributes.HasFlag(FileAttributes.Directory);
            FileSystemInfo fileSystemInfo;
            if (IsFolder)
            {
                fileSystemInfo = new DirectoryInfo(FileName);
            }
            else
            {
                var fileInfo = new FileInfo(FileName);
                fileSystemInfo = fileInfo;
                FileSize = fileInfo.Length;

            }
            CreateTime = fileSystemInfo.CreationTimeUtc;
            ModifyTime = fileSystemInfo.LastWriteTimeUtc;
        }

        public void UpdateContentHash()
        {
            using (var stream = File.OpenRead(FileName))
            {
                SHA1 sha = new SHA1CryptoServiceProvider();
                ContentHash = Convert.ToBase64String(sha.ComputeHash(stream));
            }
        }
    }
}
