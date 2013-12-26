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
        /// <summary>
        /// Updates file system properties for a file descriptor.
        /// If the file descriptor is a folder, some properties will be based on sub-folders and files.
        /// </summary>
        public void UpdateStats(FileDescriptor descriptor)
        {
            var attributes = File.GetAttributes(descriptor.FileName);
            descriptor.IsFolder = attributes.HasFlag(FileAttributes.Directory);
            FileSystemInfo fileSystemInfo;
            if (descriptor.IsFolder)
            {
                fileSystemInfo = new DirectoryInfo(descriptor.FileName);
                var childDescriptors = GetChildDescriptors(descriptor).ToList();
                foreach (var childDescriptor in childDescriptors)
                {
                    UpdateStats(childDescriptor);
                }
                descriptor.FileSize = childDescriptors.Sum(x => x.FileSize);
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

        private IEnumerable<FileDescriptor> GetChildDescriptors(FileDescriptor descriptor)
        {
            return Directory.EnumerateFileSystemEntries(descriptor.FileName).Select(entry => new FileDescriptor(entry));
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
