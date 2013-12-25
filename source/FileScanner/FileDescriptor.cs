using System;
using System.IO;

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

        public void UpdateStats()
        {
            var fileInfo = new FileInfo(FileName);
            FileSize = fileInfo.Length;
            CreateTime = fileInfo.CreationTimeUtc;
            ModifyTime = fileInfo.LastWriteTimeUtc;
        }
    }
}
