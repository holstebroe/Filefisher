
using System.Linq;

namespace FileScanner.Duplicates
{
    public class StatDuplicateComparer : IDuplicateComparer
    {

        public bool Equals(FileDescriptor x, FileDescriptor y)
        {
            return x.StatHash.SequenceEqual(y.StatHash) && x.FullPath != y.FullPath;
        }

        public int GetHashCode(FileDescriptor obj)
        {
            return obj.StatHash.Sum(x => x);
        }
    }
}