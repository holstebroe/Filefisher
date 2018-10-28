using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using FileScanner;

namespace FilefisherWpf.ViewModels
{
    public class FileSystemViewModel : ViewModelBase
    {
        private readonly MemoryFileDatabase database;
        private FileDescriptorViewModel selectedDescriptor;
        private MemoryFileDatabase referenceSystem;
        private DescriptorLookup descriptorLookup = new DescriptorLookup();
        private FilterMode filterMode;

        public FileSystemViewModel(MemoryFileDatabase database)
        {
            this.database = database;
            // Recalculate folder-sizes if zero
            database.RootDescriptor.IsFolder = true;
            database.RootDescriptor.UpdateFolderSize();

            Folders = new[] {new FolderViewModel(database.RootDescriptor, descriptorLookup)};
        }

        public IEnumerable<FolderViewModel> Folders { get; }

        public FileDescriptorViewModel SelectedDescriptor
        {
            get { return selectedDescriptor; }
            set { selectedDescriptor = value; OnPropertyChanged();}
        }

        public string RootInfo => FormatRootInfo(database.RootInfo);

        private string FormatRootInfo(RootInfo rootInfo)
        {
            if (rootInfo == null) return "not loaded";
            return $"{rootInfo.VolumeId}:{rootInfo.VolumeLabel}:{rootInfo.RootPath}";
        }

        public void UpdateReferenceSystem(MemoryFileDatabase system)
        {
            referenceSystem = system;
            descriptorLookup.Update(referenceSystem.GetAllDescriptors());
            NotifyPropertyChanged(nameof(ReferenceRootInfo));
            NotifyPropertyChanged(nameof(Folders));
        }

        public string ReferenceRootInfo => FormatRootInfo(referenceSystem?.RootInfo);

        public FilterMode FilterMode
        {
            get => filterMode;
            set
            {
                filterMode = value;
                IFileDescriptorFilter filter;
                switch (filterMode)
                {
                    case FilterMode.ShowAll:
                        filter = new ShowAllFileDescriptorFilter();
                        break;
                    case FilterMode.ShowDuplicates:
                        filter = new ShowDuplicatesFilter();
                        break;
                    case FilterMode.ShowUnique:
                        filter = new ShowUniqueFilter();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                Folders.Single().UpdateFilter(filter);
                NotifyPropertyChanged(nameof(Folders));
            }
        }
    }

    public class FileDescriptorViewModel : ViewModelBase
    {
        protected readonly FileDescriptor descriptor;
        protected DescriptorLookup descriptorLookup;

        protected FileDescriptorViewModel(FileDescriptor descriptor, DescriptorLookup lookup)
        {
            this.descriptor = descriptor;
            descriptorLookup = lookup;
        }

        public string Name => descriptor.Name;
        public string Path => descriptor.Path;

        public long Size => descriptor.Size;

        public DateTime CreateTime => descriptor.CreateTime;

        public DateTime ModifyTime => descriptor.ModifyTime;

        private string FormatDescription()
        {
            var builder = new StringBuilder();
            builder.AppendLine($"Full path: {descriptor.FullPath}");
            builder.AppendLine($"Path: {descriptor.Path}");
            builder.AppendLine($"Size: {descriptor.FormatSize()}");
            builder.AppendLine($"Created: {descriptor.CreateTime:g}");
            builder.AppendLine($"Modified: {descriptor.ModifyTime:g}");
            builder.AppendLine($"Stat hash: {FormatHash(descriptor.StatHash)}");
            builder.AppendLine($"Content hash: {FormatHash(descriptor.ContentHash)}");
            if (descriptorLookup != null)
            {
                builder.AppendLine("References by stat:");
                FormatReferenceDescriptors(descriptorLookup.LookupByStat(descriptor).ToList(), builder);

                builder.AppendLine("References by content:");
                FormatReferenceDescriptors(descriptorLookup.LookupByContent(descriptor).ToList(), builder);
            }
            return builder.ToString();
        }

        public int GetReferenceCount()
        {
            var references = descriptorLookup.LookupByStat(descriptor)
                .Concat(descriptorLookup.LookupByContent(descriptor))
                .Select(x => x.FullPath)
                .Distinct()
                .ToList();
            references.Remove(Path);
            return references.Count;
        }

        private static void FormatReferenceDescriptors(IReadOnlyCollection<FileDescriptor> referenceDescriptors, StringBuilder builder)
        {
            builder.AppendLine($"Matches: {referenceDescriptors.Count}");
            if (referenceDescriptors.Count <= 50)
                foreach (var referenceDescriptor in referenceDescriptors)
                {
                    builder.AppendLine($"Path: {referenceDescriptor.Path}");
                }
        }


        private string FormatHash(byte[] hashSignature)
        {
            return hashSignature != null ? Convert.ToBase64String(hashSignature) : "<null>";
        }

        public string MultiLineDescription
        {
            get { return FormatDescription(); }
        }

        public Brush Color
        {
            get
            {
                if (descriptorLookup == null) return null;
                var statMatch = descriptorLookup.LookupByStat(descriptor).ToList().Any();
                var contentMatch = descriptorLookup.LookupByContent(descriptor).ToList().Any();
                if (statMatch && contentMatch) return Brushes.Red;
                if (!statMatch && !contentMatch) return Brushes.Green;
                return Brushes.GreenYellow;
            }
        }

    }

    public class FolderViewModel : FileDescriptorViewModel
    {
        private readonly IEnumerable<FileDescriptorViewModel> children;
        private IEnumerable<FileDescriptorViewModel> filteredChildren;

        public FolderViewModel(FileDescriptor descriptor, DescriptorLookup lookup) : base(descriptor, lookup)
        {
            filteredChildren = children = descriptor.Children.Select(CreateViewModel).ToList();
        }
        private FileDescriptorViewModel CreateViewModel(FileDescriptor fileDescriptor)
        {
            if (fileDescriptor.IsFolder) return new FolderViewModel(fileDescriptor, descriptorLookup);
            return new FileViewModel(fileDescriptor, descriptorLookup);
        }

        public IEnumerable<FileDescriptorViewModel> Children => filteredChildren;

        public void UpdateFilter(IFileDescriptorFilter newFilter)
        {
            foreach (var child in children)
            {
                if (child is FolderViewModel folderChild)
                {
                    folderChild.UpdateFilter(newFilter);
                }
            }

            filteredChildren = children.Where(newFilter.Pass).ToList();
            NotifyPropertyChanged(nameof(Children));
        }
    }

    public class FileViewModel : FileDescriptorViewModel
    {
        public FileViewModel(FileDescriptor descriptor, DescriptorLookup lookup) : base(descriptor, lookup)
        {
        }
    }

    public interface IFileDescriptorFilter
    {
        bool Pass(FileDescriptorViewModel fileDescriptor);
    }

    public class ShowAllFileDescriptorFilter : IFileDescriptorFilter
    {
        public bool Pass(FileDescriptorViewModel fileDescriptor)
        {
            return true;
        }
    }

    public class ShowDuplicatesFilter : IFileDescriptorFilter
    {
        public bool Pass(FileDescriptorViewModel fileDescriptor)
        {
            if (fileDescriptor is FolderViewModel folderViewModel)
                return folderViewModel.Children.Any(Pass);
            return fileDescriptor.GetReferenceCount() > 0;
        }
    }
    public class ShowUniqueFilter : IFileDescriptorFilter
    {
        public bool Pass(FileDescriptorViewModel fileDescriptor)
        {
            return fileDescriptor.GetReferenceCount() == 0;
        }
    }
}
