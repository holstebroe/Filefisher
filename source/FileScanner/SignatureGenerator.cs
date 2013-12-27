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
        private readonly StatSignatureGenerator statSignatureGenerator;

        public SignatureGenerator(IHashGenerator hashGenerator)
        {
            statSignatureGenerator = new StatSignatureGenerator(hashGenerator);
        }

        /// <summary>
        /// Updates file system properties for a file descriptor.
        /// If the file descriptor is a folder, some properties will be based on sub-folders and files.
        /// </summary>
        public void UpdateStats(FileDescriptor descriptor)
        {
            var attributes = File.GetAttributes(descriptor.Path);
            descriptor.IsFolder = attributes.HasFlag(FileAttributes.Directory);
            FileSystemInfo fileSystemInfo;
            if (descriptor.IsFolder)
            {
                fileSystemInfo = new DirectoryInfo(descriptor.Path);
                var childDescriptors = GetChildDescriptors(descriptor).ToList();
                foreach (var childDescriptor in childDescriptors)
                {
                    UpdateStats(childDescriptor);
                }
                descriptor.Size = childDescriptors.Sum(x => x.Size);
            }
            else
            {
                var fileInfo = new FileInfo(descriptor.Path);
                fileSystemInfo = fileInfo;
                descriptor.Size = fileInfo.Length;

            }
            descriptor.CreateTime = fileSystemInfo.CreationTimeUtc;
            descriptor.ModifyTime = fileSystemInfo.LastWriteTimeUtc;

            statSignatureGenerator.Generate(descriptor);
        }

        private IEnumerable<FileDescriptor> GetChildDescriptors(FileDescriptor descriptor)
        {
            return Directory.EnumerateFileSystemEntries(descriptor.Path).Select(entry => new FileDescriptor(entry));
        }

        public void UpdateContentHash(FileDescriptor descriptor)
        {
            using (var stream = File.OpenRead(descriptor.Path))
            {
                SHA1 sha = new SHA1CryptoServiceProvider();
                descriptor.ContentHash = Convert.ToBase64String(sha.ComputeHash(stream));
            }
        }
    }
}
