﻿using System;
using System.Collections.Generic;
using System.Linq;
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
            Path = fullFilePath.Length > basePath.Length ? fullFilePath.Substring(basePath.Length + 1) : "";
            Name = System.IO.Path.GetFileName(fullFilePath);
            IsRoot = basePath == fullFilePath;
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
            return Path == "" ? FullPath : FullPath.Substring(0, FullPath.Length - Path.Length - 1);
        }

        public string FormatSize()
        {
            var sb = new StringBuilder(11);
            StrFormatByteSize(Size, sb, sb.Capacity);
            return sb.ToString();
        }

        [DllImport("Shlwapi.dll", CharSet = CharSet.Auto)]
        private static extern long StrFormatByteSize(long fileSize,
            [MarshalAs(UnmanagedType.LPTStr)] StringBuilder buffer, int bufferSize);

        public void UpdateFolderSize()
        {
            if (!IsFolder) return;
            if (Size != 0) return;
            if (IsFolder)
                foreach (var descriptor in Children)
                    descriptor.UpdateFolderSize();
            Size = Children.Select(x => x.Size).Sum();
        }
    }
}