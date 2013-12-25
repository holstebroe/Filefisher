using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public void UpdateStats()
        {
            var fileInfo = new FileInfo(FileName);
            FileSize = fileInfo.Length;
        }
    }
}
