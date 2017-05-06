using System.Collections.Generic;

namespace FileScanner.Duplicates
{
    interface IDuplicateFinder
    {
        IEnumerable<Duplicate> FindDuplicates(IFileDatabase databaseA, IFileDatabase databaseB);
    }
}
