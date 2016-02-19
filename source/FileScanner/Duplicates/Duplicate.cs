using System.Collections.Generic;

namespace FileScanner.Duplicates
{
    public class Duplicate
    {
        public Duplicate(IEnumerable<FileDescriptor> descriptors)
        {
            Descriptors = descriptors;
        }

        public IEnumerable<FileDescriptor> Descriptors { get; }
    }
}