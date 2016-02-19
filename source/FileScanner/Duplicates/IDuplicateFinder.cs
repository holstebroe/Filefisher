using System.Collections.Generic;

namespace FileScanner.Duplicates
{
    interface IDuplicateFinder
    {
        IEnumerable<Duplicate> Find(IFileDatabase databaseA, IFileDatabase databaseB);
    }
}
