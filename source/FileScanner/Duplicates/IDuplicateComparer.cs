using System.Collections.Generic;

namespace FileScanner.Duplicates
{
    public interface IDuplicateComparer : IEqualityComparer<FileDescriptor>
    {
    }
}