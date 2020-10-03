using System.Collections.Generic;

namespace FileScanner.Duplicates
{
    internal interface IDuplicateFinder
    {
        IEnumerable<Duplicate> FindDuplicates(IFileDatabase databaseA, IFileDatabase databaseB);
    }
}