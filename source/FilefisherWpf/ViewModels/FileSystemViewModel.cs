using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using FileScanner;

namespace FilefisherWpf.ViewModels
{
    public class FileSystemViewModel : ViewModelBase
    {
        public MemoryFileDatabase Database { get; }
        private readonly DescriptorLookup descriptorLookup = new DescriptorLookup();
        private FilterMode filterMode;
        private MemoryFileDatabase referenceSystem;
        private FileDescriptorViewModel selectedDescriptor;

        public FileSystemViewModel(MemoryFileDatabase database)
        {
            Database = database;
            // Recalculate folder-sizes if zero
            database.RootDescriptor.IsFolder = true;
            database.RootDescriptor.UpdateFolderSize();

            Folders = new[] { new FolderViewModel(database.RootDescriptor, descriptorLookup, null) };
            DeleteSelectedCommand = new RelayCommand(DeleteSelected, CanDeleteSelected);
        }

        public IEnumerable<FolderViewModel> Folders { get; }

        public FileDescriptorViewModel SelectedDescriptor
        {
            get => selectedDescriptor;
            set
            {
                selectedDescriptor = value;
                OnPropertyChanged();
            }
        }

        public string RootInfo => FormatRootInfo(Database.RootInfo);

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
                    nextDescriptor = parent.Children.SkipWhile(x => x != fileViewModel).Skip(1).FirstOrDefault();

                fileViewModel.Delete();

                if (parent != null)
                    SelectedDescriptor =
                        parent.Children.FirstOrDefault(x => x.Descriptor == nextDescriptor?.Descriptor);
            }
        }

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
    }
}