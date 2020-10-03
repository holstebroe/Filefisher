using System.Collections.Generic;
using System.Linq;

namespace FileScanner.Duplicates
{
    /// <summary>
    ///     Finds top-most duplicate in file hierarchy.
    /// </summary>
    /// <remarks>
    ///     Returns a folder pair if all children are duplicates.
    ///     Will not return child duplicates if parent is a duplicate.
    /// </remarks>
    public class TopDescriptorDuplicateFinder : IDuplicateFinder
    {
        private readonly IDuplicateComparer duplicateComparer;

        public TopDescriptorDuplicateFinder(IDuplicateComparer duplicateComparer)
        {
            this.duplicateComparer = duplicateComparer;
        }

        public bool IgnoreEmptyFolders { get; set; } = true;

        public IEnumerable<Duplicate> FindDuplicates(IFileDatabase databaseA, IFileDatabase databaseB)
        {
            var lookupB = databaseB.GetAll().ToLookup(x => x, duplicateComparer);
            var rootA = databaseA.GetRoot();
            if (rootA == null) return Enumerable.Empty<Duplicate>();
            return CheckNode(rootA, lookupB).ToList();
        }

        private IEnumerable<Duplicate> CheckNode(FileDescriptor node, ILookup<FileDescriptor, FileDescriptor> lookupB)
        {
            if (IgnoreEmptyFolders && node.IsFolder && (node.Children == null || !node.Children.Any()))
                yield break;
            if (lookupB.Contains(node))
            {
                var nodeArray = new[] {node};
                var duplicateNodes = lookupB[node].ToList();
                duplicateNodes.RemoveAll(x => x.FullPath == node.FullPath);
                if (duplicateNodes.Any())
                {
                    yield return new Duplicate(nodeArray.Concat(duplicateNodes));
                    yield break;
                }
            }

            if (node.Children != null)
                foreach (var duplicate in node.Children.SelectMany(x => CheckNode(x, lookupB)))
                    yield return duplicate;
        }
    }
}