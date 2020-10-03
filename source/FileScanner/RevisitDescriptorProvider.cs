using System.Collections.Generic;
using System.Linq;

namespace FileScanner
{
    /// <summary>
    ///     Descriptor provider that revisits the descriptor tree.
    /// </summary>
    public class RevisitDescriptorProvider : IFileDescriptorProvider
    {
        public IEnumerable<FileDescriptor> GetDirectories(FileDescriptor descriptor)
        {
            return descriptor.Children == null
                ? Enumerable.Empty<FileDescriptor>()
                : descriptor.Children.Where(x => x.IsFolder);
        }

        public IEnumerable<FileDescriptor> GetFiles(FileDescriptor descriptor)
        {
            return descriptor.Children == null
                ? Enumerable.Empty<FileDescriptor>()
                : descriptor.Children.Where(x => !x.IsFolder);
        }
    }
}