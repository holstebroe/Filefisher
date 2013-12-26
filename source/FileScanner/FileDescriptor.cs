using System;
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
        }

        public string Path { get; set; }

        public long Size { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime ModifyTime { get; set; }

        public string ContentHash { get; set; }

        public bool IsFolder { get; set; }
    }
}
