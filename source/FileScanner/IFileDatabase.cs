using System.Collections.Generic;

namespace FileScanner
{
    /// <summary>
    /// The file database contains various index structures for looking up files or folders based on different signatures,
    /// such as name, stats or content hash.
    /// </summary>
    public interface IFileDatabase
    {
        void UpdateDescriptor(FileDescriptor fileDescriptor);
        IEnumerable<FileDescriptor> GetAll();
        FileDescriptor GetRoot();
    }

}
