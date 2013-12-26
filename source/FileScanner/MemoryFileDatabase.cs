using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileScanner
{
    /// <summary>
    /// File database that will keep the entire database in memory.
    /// </summary>
    public class MemoryFileDatabase : IFileDatabase
    {
        private readonly Dictionary<string, FileDescriptor> descriptorMap = new Dictionary<string, FileDescriptor>();

        public void UpdateDescriptor(FileDescriptor fileDescriptor)
        {
            descriptorMap.Add(fileDescriptor.Path, fileDescriptor);
        }

        /// <summary>
        /// Returns all file descriptors in the database.
        /// </summary>
        public IEnumerable<FileDescriptor> GetAllDescriptors()
        {
            return descriptorMap.Values;
        }
    }
}
