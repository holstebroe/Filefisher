using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileScanner
{
    /// <summary>
    ///     File descriptor provider that uses the operating file system
    /// </summary>
    public class SystemFileDescriptorProvider : IFileDescriptorProvider
    {
        public IEnumerable<FileDescriptor> GetDirectories(FileDescriptor descriptor)
        {
            var directoryInfo = new DirectoryInfo(descriptor.FullPath);
            var subDirectories = directoryInfo.EnumerateDirectories("*.*", SearchOption.TopDirectoryOnly);
            var basePath = descriptor.GetBasePath();
            return subDirectories.Select(x => CreateFolderDescriptor(basePath, x));
        }

        public IEnumerable<FileDescriptor> GetFiles(FileDescriptor descriptor)
        {
            var directoryInfo = new DirectoryInfo(descriptor.FullPath);
            var subDirectories = directoryInfo.EnumerateFiles("*.*", SearchOption.TopDirectoryOnly);
            var basePath = descriptor.GetBasePath();
            return subDirectories.Select(x => CreateFileDescriptor(basePath, x)).Where(x => x != null);
        }

        /// <summary>
        ///     Creates file descriptor with file system properties.
        /// </summary>
        private FileDescriptor CreateFileDescriptor(string basePath, FileInfo fileInfo)
        {
            string fullName;
            try
            {
                fullName = fileInfo.FullName;
            }
            // File name not supported, could be caused by a mounted linux drive with filenames not valid in Windows.
            catch (NotSupportedException)
            {
                return null;
            }

            return new FileDescriptor(basePath, fullName)
            {
                IsFolder = false,
                CreateTime = fileInfo.CreationTimeUtc,
                ModifyTime = fileInfo.LastWriteTimeUtc,
                Size = fileInfo.Length
            };
        }

        /// <summary>
        ///     Updates file system properties for a file descriptor.
        ///     If the file descriptor is a folder, some properties will be based on sub-folders and files.
        /// </summary>
        private FileDescriptor CreateFolderDescriptor(string basePath, FileSystemInfo fileInfo)
        {
            var descriptor = new FileDescriptor(basePath, fileInfo.FullName)
            {
                IsFolder = true,
                CreateTime = fileInfo.CreationTimeUtc,
                ModifyTime = fileInfo.LastWriteTimeUtc
            };
            return descriptor;
        }
    }
}