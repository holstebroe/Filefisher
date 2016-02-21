using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace FileScanner
{
    [Serializable]
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

        public byte[] ContentHash { get; set; }

        public bool IsFolder { get; set; }

        public bool IsRoot { get; set; }

        public IEnumerable<FileDescriptor> Children { get; set; }

        public string GetBasePath()
        {
            return FullPath.Substring(0, FullPath.Length - Path.Length);
        }

        public string FormatSize()
        {
            StringBuilder sb = new StringBuilder(11);
            StrFormatByteSize(Size, sb, sb.Capacity);
            return sb.ToString();
        }
        [DllImport("Shlwapi.dll", CharSet = CharSet.Auto)]
        private static extern long StrFormatByteSize(long fileSize, [MarshalAs(UnmanagedType.LPTStr)] StringBuilder buffer, int bufferSize);
    }
}
