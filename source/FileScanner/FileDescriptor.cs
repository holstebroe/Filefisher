using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace FileScanner
{
    public class FileDescriptor
    {
        public FileDescriptor(string path)
        {
            Path = path;
            Name = System.IO.Path.GetFileName(path);
        }

        public string Path { get; set; }

        public string Name { get; set; }

        public long Size { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime ModifyTime { get; set; }

        public byte[] StatHash { get; set; }

        public string ContentHash { get; set; }

        public bool IsFolder { get; set; }

        public IEnumerable<FileDescriptor> Children { get; set; }
    }
}
