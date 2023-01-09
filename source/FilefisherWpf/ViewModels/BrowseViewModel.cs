using System.Collections.Generic;
using System.Windows.Input;
using FileScanner;
using Microsoft.Win32;

namespace FilefisherWpf.ViewModels
{
    public class BrowseViewModel : ViewModelBase
    {
        private FileSystemViewModel fileSystemViewModel;
        private FilterMode selectedFilter;
        private FileSystemViewModel referenceFileSystemViewModel;

        public BrowseViewModel()
        {
            LoadFileSystemCommand = new RelayCommand(LoadFileSystem);
            LoadReferenceSystemCommand = new RelayCommand(LoadReferenceSystem);

            selectedFilter = FilterMode.ShowAll;
            FilterItems = new[]
            {
                new FilterItem("Show all", FilterMode.ShowAll),
                new FilterItem("Show duplicates", FilterMode.ShowDuplicates),
                new FilterItem("Show unique", FilterMode.ShowUnique)
            };
        }

        public ICommand LoadFileSystemCommand { get; }

        public FileSystemViewModel FileSystemViewModel
        {
            get => fileSystemViewModel;
            set
            {
                fileSystemViewModel = value;
                OnPropertyChanged();
            }
        }

        public FileSystemViewModel ReferenceFileSystemViewModel
        {
            get => referenceFileSystemViewModel;
            set
            {
                if (Equals(value, referenceFileSystemViewModel)) return;
                referenceFileSystemViewModel = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoadReferenceSystemCommand { get; }

        public IEnumerable<FilterItem> FilterItems { get; }

        public FilterMode SelectedFilter
        {
            get => selectedFilter;
            set
            {
                selectedFilter = value;
                if (FileSystemViewModel != null)
                    FileSystemViewModel.FilterMode = SelectedFilter;
                OnPropertyChanged();
            }
        }

        private void LoadFileSystem()
        {
            var dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                var databaseFile = dialog.FileName;
                var database = MemoryFileDatabase.Load(databaseFile);
                FileSystemViewModel = new FileSystemViewModel(database);
                if (ReferenceFileSystemViewModel == null)
                {
                    FileSystemViewModel.UpdateReferenceSystem(database);
                    ReferenceFileSystemViewModel = new FileSystemViewModel(database);
                }
                else
                {
                    FileSystemViewModel.UpdateReferenceSystem(ReferenceFileSystemViewModel.Database);
                }

                FileSystemViewModel.FilterMode = SelectedFilter;
            }
        }

        private void LoadReferenceSystem()
        {
            var dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                var databaseFile = dialog.FileName;
                var database = MemoryFileDatabase.Load(databaseFile);
                ReferenceFileSystemViewModel = new FileSystemViewModel(database);
                FileSystemViewModel.UpdateReferenceSystem(database);
            }
        }
    }

    public enum FilterMode
    {
        ShowAll,
        ShowDuplicates,
        ShowUnique
    }

    public class FilterItem

    {
        public FilterItem(string text, object value)
        {
            Text = text;
            Value = value;
        }

        public string Text { get; }
        public object Value { get; }
    }
}