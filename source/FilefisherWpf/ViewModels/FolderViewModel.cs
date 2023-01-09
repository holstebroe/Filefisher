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

        public FolderViewModel(FileDescriptor descriptor, DescriptorLookup lookup, FolderViewModel parent) : base(
            descriptor, lookup, parent)
        {
            children = descriptor.Children.Select(CreateChildViewModel).ToList();
            filteredChildren = new ObservableCollection<FileDescriptorViewModel>(children);
        }

        public IEnumerable<FileDescriptorViewModel> Children => filteredChildren;

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
    }
}