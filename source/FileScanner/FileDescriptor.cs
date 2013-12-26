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

        public string FileName { get; set; }

        public long FileSize { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime ModifyTime { get; set; }

        public string ContentHash { get; set; }

        public bool IsFolder { get; set; }
    }
}
