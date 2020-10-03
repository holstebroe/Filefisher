using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Media;
using FileScanner;
using Microsoft.VisualBasic.FileIO;

namespace FilefisherWpf.ViewModels
{
    public class FileSystemViewModel : ViewModelBase
    {
        private readonly MemoryFileDatabase database;
        private FileDescriptorViewModel selectedDescriptor;
        private MemoryFileDatabase referenceSystem;
        private readonly DescriptorLookup descriptorLookup = new DescriptorLookup();
        private FilterMode filterMode;

        public FileSystemViewModel(MemoryFileDatabase database)
        {
            this.database = database;
            // Recalculate folder-sizes if zero
            database.RootDescriptor.IsFolder = true;
            database.RootDescriptor.UpdateFolderSize();

            Folders = new[] {new FolderViewModel(database.RootDescriptor, descriptorLookup, null)};
            DeleteSelectedCommand = new RelayCommand(DeleteSelected, CanDeleteSelected);
        }

        private bool CanDeleteSelected()
        {
            return SelectedDescriptor is FileViewModel && SelectedDescriptor.Exists;
        }

        private void DeleteSelected()
        {
            if (SelectedDescriptor is FileViewModel fileViewModel)
            {
                var parent = fileViewModel.Parent;
                FileDescriptorViewModel nextDescriptor = null;
                // Select next
                if (parent != null)
                {
                    nextDescriptor = parent.Children.SkipWhile(x => x != fileViewModel).Skip(1).FirstOrDefault();
                }

                fileViewModel.Delete();

                if (parent != null)
                    SelectedDescriptor = parent.Children.FirstOrDefault(x => x.Descriptor == nextDescriptor?.Descriptor);
            }
        }

        public IEnumerable<FolderViewModel> Folders { get; }

        public FileDescriptorViewModel SelectedDescriptor
        {
            get => selectedDescriptor;
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

        public ICommand DeleteSelectedCommand { get; }
    }

    public class FileDescriptorViewModel : ViewModelBase
    {
        public FileDescriptor Descriptor { get; }
        protected DescriptorLookup descriptorLookup;
        public FolderViewModel Parent { get; }

        protected FileDescriptorViewModel(FileDescriptor descriptor, DescriptorLookup lookup, FolderViewModel parent)
        {
            this.Descriptor = descriptor;
            descriptorLookup = lookup;
            this.Parent = parent;
        }

        public string Name => Descriptor.Name;
        public string Path => Descriptor.FullPath;

        public long Size => Descriptor.Size;

        public DateTime CreateTime => Descriptor.CreateTime;

        public DateTime ModifyTime => Descriptor.ModifyTime;

        public bool Exists => File.Exists(Descriptor.FullPath);

        private string FormatDescription()
        {
            var builder = new StringBuilder();
            builder.AppendLine($"Full path: {Descriptor.FullPath}");
            builder.AppendLine($"Path: {Descriptor.Path}");
            builder.AppendLine($"Size: {Descriptor.FormatSize()}");
            builder.AppendLine($"Created: {Descriptor.CreateTime:g}");
            builder.AppendLine($"Modified: {Descriptor.ModifyTime:g}");
            builder.AppendLine($"Stat hash: {FormatHash(Descriptor.StatHash)}");
            builder.AppendLine($"Content hash: {FormatHash(Descriptor.ContentHash)}");
            if (descriptorLookup != null)
            {
                builder.AppendLine("References by stat:");
                FormatReferenceDescriptors(ExceptThis(descriptorLookup.LookupByStat(Descriptor).DistinctBy(x => x.FullPath)).ToList(), builder);

                builder.AppendLine("References by content:");
                FormatReferenceDescriptors(ExceptThis(descriptorLookup.LookupByContent(Descriptor).DistinctBy(x => x.FullPath)).ToList(), builder);
            }
            return builder.ToString();
        }

        public IEnumerable<FileDuplicateViewModel> Duplicates
        {
            get
            {
                if (descriptorLookup == null) yield break;
                var statDuplicates = descriptorLookup.LookupByStat(Descriptor).DistinctBy(x => x.FullPath).ToList();
                var contentDuplicates = descriptorLookup.LookupByContent(Descriptor).DistinctBy(x => x.FullPath).ToList();
                var allDuplicates = statDuplicates.Concat(contentDuplicates).DistinctBy(x => x.FullPath);
                var statDuplicatePaths = new HashSet<string>(statDuplicates.Select(x => x.FullPath));
                var contentDuplicatePaths = new HashSet<string>(contentDuplicates.Select(x => x.FullPath));

                foreach (var fileDescriptor in allDuplicates.Where(x => x.Size >= 0))
                {
                    yield return new FileDuplicateViewModel(fileDescriptor, statDuplicatePaths.Contains(fileDescriptor.FullPath), contentDuplicatePaths.Contains(fileDescriptor.FullPath));
                }
            }
        }

        private IEnumerable<FileDescriptor> ExceptThis(IEnumerable<FileDescriptor> descriptors)
        {
            return descriptors.Where(other => other.FullPath != Descriptor.FullPath);
        }

        public int GetReferenceCount()
        {
            var references = descriptorLookup.LookupByStat(Descriptor)
                .Concat(descriptorLookup.LookupByContent(Descriptor))
                .Where(x => x.Size > 0)
                .Select(x => x.FullPath)
                .Distinct()
                .ToList();
            references.Remove(Descriptor.FullPath);
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

        public string ImagePath       
        {
            get
            {
                var ext = System.IO.Path.GetExtension(Descriptor.FullPath);
                if (ext == null) return null;
                ext = ext.ToLower();
                if (ext == ".jpg" || ext == ".png" || ext == ".gif" || ext == ".bmp" || ext == ".jpeg")
                    return Descriptor.FullPath;
                return null;
            }
        }

        public Brush Color
        {
            get
            {
                if (descriptorLookup == null) return null;
                var statMatch = descriptorLookup.LookupByStat(Descriptor).ToList().Any();
                var contentMatch = descriptorLookup.LookupByContent(Descriptor).ToList().Any();
                if (statMatch && contentMatch) return Brushes.Red;
                if (!statMatch && !contentMatch) return Brushes.Green;
                return Brushes.GreenYellow;
            }
        }
    }

    public class FileDuplicateViewModel : ViewModelBase
    {
        public FileDuplicateViewModel(FileDescriptor descriptor, bool matchStat, bool matchContent)
        {
            Descriptor = descriptor;
            MatchStat = matchStat;
            MatchContent = matchContent;
            DeleteCommand = new RelayCommand(DeleteFile, CanDeleteFile);
        }

        private bool CanDeleteFile()
        {
            return File.Exists(Descriptor.FullPath);
        }

        private void DeleteFile()
        {
            File.Delete(Descriptor.FullPath);
        }

        public ICommand DeleteCommand { get; }

        public FileDescriptor Descriptor { get; }
        public bool MatchStat { get; }
        public bool MatchContent { get; }
    }

    public class FolderViewModel : FileDescriptorViewModel
    {
        private readonly IList<FileDescriptorViewModel> children;
        private ObservableCollection<FileDescriptorViewModel> filteredChildren;
        private IFileDescriptorFilter filter;

        public FolderViewModel(FileDescriptor descriptor, DescriptorLookup lookup, FolderViewModel parent) : base(descriptor, lookup, parent)
        {
            children = descriptor.Children.Select(CreateChildViewModel).ToList();
            filteredChildren = new ObservableCollection<FileDescriptorViewModel>(children);
        }
        private FileDescriptorViewModel CreateChildViewModel(FileDescriptor fileDescriptor)
        {
            if (fileDescriptor.IsFolder) return new FolderViewModel(fileDescriptor, descriptorLookup, this);
            return new FileViewModel(fileDescriptor, descriptorLookup, this);
        }

        public IEnumerable<FileDescriptorViewModel> Children => filteredChildren;



        public void UpdateFilter(IFileDescriptorFilter newFilter)
        {
            filter = newFilter;
            Update();
        }

        public void Update()
        {
            foreach (var child in children)
            {
                if (child is FolderViewModel folderChild)
                {
                    folderChild.UpdateFilter(filter);
                }
            }

            filteredChildren = new ObservableCollection<FileDescriptorViewModel>(children.Where(filter.Pass).ToList());
            NotifyPropertyChanged(nameof(Children));
        }

        public void Remove(FileViewModel fileViewModel)
        {
            children.Remove(fileViewModel);
            filteredChildren.Remove(fileViewModel);
            NotifyPropertyChanged(nameof(Children));
        }
    }

    public class FileViewModel : FileDescriptorViewModel
    {

        public FileViewModel(FileDescriptor descriptor, DescriptorLookup lookup, FolderViewModel parent) : base(descriptor, lookup, parent)
        {
        }

        public void Delete()
        {
            FileSystem.DeleteFile(Path, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
            Descriptor.Size = -1;

            var duplicates = Duplicates.Select(x => x.Descriptor).ToList();
            var root = Parent;
            if (root != null)
                while (root.Parent != null) root = root.Parent;

            Parent.Remove(this);
            foreach (var duplicate in duplicates)
            {
                FindAndUpdateDescriptor(root, duplicate);
            }
        }

        private void FindAndUpdateDescriptor(FolderViewModel node, FileDescriptor fileDescriptor)
        {
            if (node == null) return;
            foreach (var child in node.Children)
            {
                if (child.Descriptor == fileDescriptor)
                {
                    child.Parent?.Update();
                    return;
                }
                if (child is FolderViewModel folderViewModel)
                    FindAndUpdateDescriptor(folderViewModel, fileDescriptor);
            }
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

    public class SelectorEqualityComparer<TA, TB> : IEqualityComparer<TA>
    {
        private readonly Func<TA, TB> selector;

        public SelectorEqualityComparer(Func<TA, TB> selector)
        {
            this.selector = selector;
        }

        public bool Equals(TA x, TA y)
        {
            if (object.Equals(x, default(TA)) && object.Equals(y, default(TA))) return true;
            if (object.Equals(x, default(TA))) return false;
            if (object.Equals(y, default(TA))) return false;

            return EqualityComparer<TB>.Default.Equals(selector(x), selector(y));
        }

        public int GetHashCode(TA obj)
        {
            if (Equals(obj, default(TA))) return 0;
            var value = selector(obj);
            if (Equals(value, default(TB))) return 0;
            return value.GetHashCode();
        }
    }


    public static class LinqExtensions
    {
        public static IEnumerable<TItem> DistinctBy<TItem, TKey>(this IEnumerable<TItem> items,
            Func<TItem, TKey> keySelector)
        {
            return items.Distinct(new SelectorEqualityComparer<TItem, TKey>(keySelector));
        }
        /// <summary>
        /// Filters a collection of items by another set using a specified comparison key selector.
        /// </summary>
        public static IEnumerable<TItem> ExceptBy<TItem, TKey>(this IEnumerable<TItem> items,
            IEnumerable<TItem> excludeSet, Func<TItem, TKey> keySelector)
        {
            return items.Except(excludeSet, new SelectorEqualityComparer<TItem, TKey>(keySelector));
        }

        /// <summary>
        /// Filters a collection of items by a predicate. Same as negation of Where.
        /// </summary>
        public static IEnumerable<TItem> Except<TItem>(this IEnumerable<TItem> items, Predicate<TItem> predicate)
        {
            return items.Where(x => !predicate(x));
        }
    }
}
