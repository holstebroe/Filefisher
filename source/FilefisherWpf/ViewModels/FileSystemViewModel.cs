using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileScanner;

namespace FilefisherWpf.ViewModels
{
    public class FileSystemViewModel : ViewModelBase
    {
        private readonly MemoryFileDatabase database;
        private FileDescriptorViewModel selectedDescriptor;

        public FileSystemViewModel(MemoryFileDatabase database)
        {
            this.database = database;
            // Recalculate folder-sizes if zero
            database.RootDescriptor.IsFolder = true;
            UpdateFolderSize(database.RootDescriptor);
            Folders = new[] {new FolderViewModel(database.RootDescriptor)};
        }

        private void UpdateFolderSize(FileDescriptor fileDescriptor)
        {
            if (!fileDescriptor.IsFolder) return;
            if (fileDescriptor.Size != 0) return;
            if (fileDescriptor.IsFolder)
                foreach (var descriptor in fileDescriptor.Children)
                {
                    UpdateFolderSize(descriptor);
                }
            fileDescriptor.Size = fileDescriptor.Children.Select(x => x.Size).Sum();
        }
        public IEnumerable<FolderViewModel> Folders { get; }

        public FileDescriptorViewModel SelectedDescriptor
        {
            get { return selectedDescriptor; }
            set { selectedDescriptor = value; OnPropertyChanged();}
        }
    }

    public class FileDescriptorViewModel
    {
        private readonly FileDescriptor descriptor;

        protected FileDescriptorViewModel(FileDescriptor descriptor)
        {
            this.descriptor = descriptor;
        }


        public string Name => descriptor.Name;
        public string Path => descriptor.Path;

        public long Size => descriptor.Size;

        public DateTime CreateTime => descriptor.CreateTime;

        public DateTime ModifyTime => descriptor.ModifyTime;
    }

    public class FolderViewModel : FileDescriptorViewModel
    {
        public FolderViewModel(FileDescriptor descriptor) : base(descriptor)
        {
            Children = descriptor.Children.Select(CreateFileDescriptor);
            MultiLineDescription = FormatDescription(descriptor);
        }
        private FileDescriptorViewModel CreateFileDescriptor(FileDescriptor fileDescriptor)
        {
            if (fileDescriptor.IsFolder) return new FolderViewModel(fileDescriptor);
            return new FileViewModel(fileDescriptor);
        }
        public IEnumerable<FileDescriptorViewModel> Children { get; }


        public string MultiLineDescription { get; }


        private string FormatDescription(FileDescriptor descriptor)
        {
            var builder = new StringBuilder();
            builder.AppendLine($"Path: {descriptor.FullPath}");
            builder.AppendLine($"Size: {descriptor.FormatSize()}");
            builder.AppendLine($"Created: {descriptor.CreateTime:g}");
            builder.AppendLine($"Modified: {descriptor.ModifyTime:g}");
            builder.AppendLine($"Stat hash: {FormatHash(descriptor.StatHash)}");
            builder.AppendLine($"Content hash: {FormatHash(descriptor.ContentHash)}");
            return builder.ToString();
        }

        private string FormatHash(byte[] hashSignature)
        {
            return hashSignature != null ? Convert.ToBase64String(hashSignature) : "<null>";
        }


    }

    public class FileViewModel : FileDescriptorViewModel
    {
        public FileViewModel(FileDescriptor descriptor) : base(descriptor)
        {
            MultiLineDescription = FormatDescription(descriptor);
        }

        private string FormatDescription(FileDescriptor descriptor)
        {
            var builder = new StringBuilder();
            builder.AppendLine($"Path: {descriptor.FullPath}");
            builder.AppendLine($"Size: {descriptor.FormatSize()}");
            builder.AppendLine($"Created: {descriptor.CreateTime:g}");
            builder.AppendLine($"Modified: {descriptor.ModifyTime:g}");
            builder.AppendLine($"Stat hash: {FormatHash(descriptor.StatHash)}");
            builder.AppendLine($"Content hash: {FormatHash(descriptor.ContentHash)}");
            return builder.ToString();
        }

        private string FormatHash(byte[] hashSignature)
        {
            return hashSignature != null ? Convert.ToBase64String(hashSignature) : "<null>";
        }

        public string MultiLineDescription { get; }
    }
}
