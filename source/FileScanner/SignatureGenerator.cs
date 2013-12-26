using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace FileScanner
{
    /// <summary>
    /// Class for generating file or folder signatures.
    /// </summary>
    public class SignatureGenerator
    {
        public void UpdateStats(FileDescriptor descriptor)
        {
            var attributes = File.GetAttributes(descriptor.FileName);
            descriptor.IsFolder = attributes.HasFlag(FileAttributes.Directory);
            FileSystemInfo fileSystemInfo;
            if (descriptor.IsFolder)
            {
                fileSystemInfo = new DirectoryInfo(descriptor.FileName);
            }
            else
            {
                var fileInfo = new FileInfo(descriptor.FileName);
                fileSystemInfo = fileInfo;
                descriptor.FileSize = fileInfo.Length;

            }
            descriptor.CreateTime = fileSystemInfo.CreationTimeUtc;
            descriptor.ModifyTime = fileSystemInfo.LastWriteTimeUtc;
        }

        public void UpdateContentHash(FileDescriptor descriptor)
        {
            using (var stream = File.OpenRead(descriptor.FileName))
            {
                SHA1 sha = new SHA1CryptoServiceProvider();
                descriptor.ContentHash = Convert.ToBase64String(sha.ComputeHash(stream));
            }
        }
    }
}
