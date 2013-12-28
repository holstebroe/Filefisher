using System.Collections.Generic;

namespace FileScanner
{
    /// <summary>
    /// Interface for providers of file descriptors, which could be the underlying file system itself.
    /// </summary>
    public interface IFileDescriptorProvider
    {
        /// <summary>
        /// Returns all directories directly under the directory represented by the file descriptor.
        /// </summary>
        IEnumerable<FileDescriptor> GetDirectories(FileDescriptor descriptor);

        /// <summary>
        /// Returns all files directly under the directory represented by the file descriptor.
        /// </summary>
        IEnumerable<FileDescriptor> GetFiles(FileDescriptor descriptor);
    }
}
