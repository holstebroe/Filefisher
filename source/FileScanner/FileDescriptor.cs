using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace FileScanner
{
    public class FileDescriptor
    {
        public FileDescriptor(string relativeFilePath)
        {
            FullPath = System.IO.Path.GetFullPath(relativeFilePath);
            Path = FullPath.Substring(FullPath.Length - relativeFilePath.Length);
            Name = System.IO.Path.GetFileName(Path);

        }
        public FileDescriptor(string basePath, string fullFilePath)
        {
            FullPath = fullFilePath;
            Path = fullFilePath.Substring(basePath.Length);
            Name = System.IO.Path.GetFileName(fullFilePath);
        }

        public string FullPath { get; set; }

        public string Path { get; set; }

        public string Name { get; set; }

        public long Size { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime ModifyTime { get; set; }

        public byte[] StatHash { get; set; }

        public string ContentHash { get; set; }

        public bool IsFolder { get; set; }

        public IEnumerable<FileDescriptor> Children { get; set; }

        public string GetBasePath()
        {
            return FullPath.Substring(0, FullPath.Length - Path.Length);
        }
    }
}
