using System.Collections.Generic;
using System.Linq;

namespace FileScanner.Duplicates
{
    public class FileDuplicateFinder : IDuplicateFinder
    {
        private readonly IDuplicateComparer duplicateComparer;

        public FileDuplicateFinder(IDuplicateComparer duplicateComparer)
        {
            this.duplicateComparer = duplicateComparer;
        }

        public IEnumerable<Duplicate> FindDuplicates(IFileDatabase databaseA, IFileDatabase databaseB)
        {
            var allADescriptors = databaseA.GetAll();
            var allBDescriptors = databaseB.GetAll();

            var joinedDescriptors = allADescriptors.Join(allBDescriptors, x => x, x => x,
                (fda, fdb) => new Duplicate(new[] {fda, fdb}), duplicateComparer);

            return joinedDescriptors.ToList();
        }

        public IEnumerable<FileDescriptor> FindUniqueA(IFileDatabase databaseA, IFileDatabase databaseB)
        {
            var allADescriptors = databaseA.GetAll();
            var allBDescriptors = databaseB.GetAll();

            var joinedDescriptors = allADescriptors.Except(allBDescriptors, duplicateComparer);

            return joinedDescriptors.ToList();
        }
    }
}