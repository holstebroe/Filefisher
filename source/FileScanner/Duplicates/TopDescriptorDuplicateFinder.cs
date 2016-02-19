using System;
using System.Collections.Generic;
using System.Linq;

namespace FileScanner.Duplicates
{
    /// <summary>
    /// Finds top-most duplicate in file hierarchy.
    /// </summary>
    /// <remarks>
    /// Returns a folder pair if all children are duplicates.
    /// Will not return child duplicates if parent is a duplicate.
    /// </remarks>
    public class TopDescriptorDuplicateFinder : IDuplicateFinder
    {
        private IDuplicateComparer duplicateComparer;

        public TopDescriptorDuplicateFinder(IDuplicateComparer duplicateComparer)
        {
            this.duplicateComparer = duplicateComparer;
        }

        public IEnumerable<Duplicate> Find(IFileDatabase databaseA, IFileDatabase databaseB)
        {
            var lookupB = databaseB.GetAll().ToLookup(x => x, duplicateComparer);
            var rootA = databaseA.GetRoot();
            if (rootA == null) return Enumerable.Empty<Duplicate>();
            return FindChild(rootA, lookupB).ToList();
        }

        private IEnumerable<Duplicate> FindChild(FileDescriptor node, ILookup<FileDescriptor, FileDescriptor> lookupB)
        {
            if (node.Children == null) yield break;
            if (lookupB.Contains(node)) yield return new Duplicate(new [] {node}.Concat(lookupB[node]));
            else
            foreach (var duplicate in node.Children.SelectMany(x => FindChild(x, lookupB)))
            {
                yield return duplicate;
            }
        }
    }
}
