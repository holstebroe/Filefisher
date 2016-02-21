using System.Linq;

namespace FileScanner.Duplicates
{
    public class ContentDuplicateComparer : IDuplicateComparer
    {
        public bool Equals(FileDescriptor x, FileDescriptor y)
        {
            return x.ContentHash.SequenceEqual(y.ContentHash) && x.FullPath != y.FullPath;
        }

        public int GetHashCode(FileDescriptor obj)
        {
            return obj.ContentHash.Sum(x => x);
        }
    }
}