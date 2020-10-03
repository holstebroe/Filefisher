using System.Linq;

namespace FileScanner.Duplicates
{
    public class ContentDuplicateComparer : IDuplicateComparer
    {
        public bool Equals(FileDescriptor x, FileDescriptor y)
        {
            if (x == null && y == null) return false;
            if (x == null || y == null) return false;
            if (x.ContentHash == null || y.ContentHash == null) return false;
            if (x.Size == 0 || y.Size == 0) return false;
            return x.ContentHash.SequenceEqual(y.ContentHash) && x.FullPath != y.FullPath;
        }

        public int GetHashCode(FileDescriptor obj)
        {
            if (obj.ContentHash == null) return -1;
            return obj.ContentHash.Sum(x => x);
        }
    }
}