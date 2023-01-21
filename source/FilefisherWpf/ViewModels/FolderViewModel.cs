using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FileScanner;

namespace FilefisherWpf.ViewModels
{
    public class FolderViewModel : FileDescriptorViewModel
    {
        private readonly IList<FileDescriptorViewModel> children;
        private IFileDescriptorFilter filter;
        private ObservableCollection<FileDescriptorViewModel> filteredChildren;
        private bool isExpanded;
        private bool isSelected;

        public FolderViewModel(FileDescriptor descriptor, DescriptorLookup lookup, FolderViewModel parent) : base(
            descriptor, lookup, parent)
        {
            children = descriptor.Children.Select(CreateChildViewModel).ToList();
            filteredChildren = new ObservableCollection<FileDescriptorViewModel>(children);
        }

        public IEnumerable<FileDescriptorViewModel> Children => filteredChildren;

        public IEnumerable<FileViewModel> FileChildren => filteredChildren.OfType<FileViewModel>();
        public IEnumerable<FolderViewModel> FolderChildren => filteredChildren.OfType<FolderViewModel>();

        public bool IsExpanded
        {
            get => isExpanded;
            set
            {
                if (value == isExpanded) return;
                isExpanded = value;
                OnPropertyChanged();
            }
        }

        private FileDescriptorViewModel CreateChildViewModel(FileDescriptor fileDescriptor)
        {
            if (fileDescriptor.IsFolder) return new FolderViewModel(fileDescriptor, DescriptorLookup, this);
            return new FileViewModel(fileDescriptor, DescriptorLookup, this);
        }


        public void UpdateFilter(IFileDescriptorFilter newFilter)
        {
            filter = newFilter;
            Update();
        }

        public void Update()
        {
            foreach (var child in children)
                if (child is FolderViewModel folderChild)
                    folderChild.UpdateFilter(filter);

            filteredChildren = new ObservableCollection<FileDescriptorViewModel>(children.Where(filter.Pass).ToList());
            NotifyPropertyChanged(nameof(Children));
        }

        public void Remove(FileViewModel fileViewModel)
        {
            children.Remove(fileViewModel);
            filteredChildren.Remove(fileViewModel);
            NotifyPropertyChanged(nameof(Children));
        }

        public FileDescriptorViewModel FindDescriptorViewModel(FileDescriptor fileDescriptor)
        {
            if (fileDescriptor == null) return null;
            foreach (var child in FileChildren) child.IsSelected = false;
            var fileMatch = (FileDescriptorViewModel) FileChildren.FirstOrDefault(x => x.Descriptor.FullPath == fileDescriptor.FullPath);
            if (fileMatch != null)
            {
                fileMatch.IsSelected = true;
            }
            else
            {
                // No match, try children
                IsSelected = false;
                fileMatch = FolderChildren.Select(x => x.FindDescriptorViewModel(fileDescriptor)).FirstOrDefault(x => x != null);
            }

            IsExpanded = fileMatch != null;

            return fileMatch;
        }
    }
}