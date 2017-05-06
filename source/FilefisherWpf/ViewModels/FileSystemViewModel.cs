using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileScanner;

namespace FilefisherWpf.ViewModels
{
    public class FileSystemViewModel
    {
        private readonly MemoryFileDatabase database;

        public FileSystemViewModel(MemoryFileDatabase database)
        {
            this.database = database;
            Folders = new[] {new FolderViewModel(database.RootDescriptor)};
        }

        public IEnumerable<FolderViewModel> Folders { get; }
    }

    public class FileDescriptorViewModel
    {
        private readonly FileDescriptor descriptor;

        protected FileDescriptorViewModel(FileDescriptor descriptor)
        {
            this.descriptor = descriptor;
        }


        public string Name => descriptor.Name;

    }

    public class FolderViewModel : FileDescriptorViewModel
    {
        public FolderViewModel(FileDescriptor descriptor) : base(descriptor)
        {
            Children = descriptor.Children.Select(CreateFileDescriptor);
        }
        private FileDescriptorViewModel CreateFileDescriptor(FileDescriptor fileDescriptor)
        {
            if (fileDescriptor.IsFolder) return new FolderViewModel(fileDescriptor);
            return new FileViewModel(fileDescriptor);
        }
        public IEnumerable<FileDescriptorViewModel> Children { get; }

    }

    public class FileViewModel : FileDescriptorViewModel
    {
        public FileViewModel(FileDescriptor descriptor) : base(descriptor)
        {
        }
    }
}
