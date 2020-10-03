using System.Collections.Generic;
using System.Linq;

namespace FileScanner
{
    /// <summary>
    ///     Dummy database that ignores all method calls.
    /// </summary>
    public class NullFileDatabase : IFileDatabase
    {
        public void UpdateDescriptor(FileDescriptor fileDescriptor)
        {
            // Does nothing
        }

        public IEnumerable<FileDescriptor> GetAll()
        {
            return Enumerable.Empty<FileDescriptor>();
        }

        public FileDescriptor GetRoot()
        {
            return null;
        }
    }
}