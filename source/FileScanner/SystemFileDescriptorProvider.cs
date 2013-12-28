using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FileScanner
{
    /// <summary>
    /// File descriptor provider that uses the operating file system
    /// </summary>
    public class SystemFileDescriptorProvider : IFileDescriptorProvider
    {

        public IEnumerable<FileDescriptor> GetDirectories(FileDescriptor descriptor)
        {
            var directoryInfo = new DirectoryInfo(descriptor.FullPath);
            var subDirectories = directoryInfo.EnumerateDirectories("*.*", SearchOption.TopDirectoryOnly);
            return subDirectories.Select(x => new FileDescriptor(descriptor.GetBasePath(), x.FullName));
        }

        public IEnumerable<FileDescriptor> GetFiles(FileDescriptor descriptor)
        {
            var directoryInfo = new DirectoryInfo(descriptor.FullPath);
            var subDirectories = directoryInfo.EnumerateFiles("*.*", SearchOption.TopDirectoryOnly);
            return subDirectories.Select(x => new FileDescriptor(descriptor.GetBasePath(), x.FullName));
        }
    }
}
