using System.Collections.Generic;
using System.Windows.Input;
using FileScanner;
using Microsoft.Win32;

namespace FilefisherWpf.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private FileSystemViewModel fileSystemViewModel;
        private FilterMode selectedFilter;

        public MainViewModel()
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
        private void LoadFileSystem()
        {
            var dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                var databaseFile = dialog.FileName;
                var database = MemoryFileDatabase.Load(databaseFile);
                FileSystemViewModel = new FileSystemViewModel(database);
                FileSystemViewModel.UpdateReferenceSystem(database);
                FileSystemViewModel.FilterMode = SelectedFilter;
            }
        }

        public FileSystemViewModel FileSystemViewModel
        {
            get { return fileSystemViewModel; }
            set { fileSystemViewModel = value; OnPropertyChanged();}
        }

        public ICommand LoadReferenceSystemCommand { get; }

        public IEnumerable<FilterItem> FilterItems { get; private set;}

        public FilterMode SelectedFilter
        {
            get => selectedFilter;
            set { selectedFilter = value;
                if (FileSystemViewModel != null)
                    FileSystemViewModel.FilterMode = SelectedFilter;
                OnPropertyChanged();}
        }

        private void LoadReferenceSystem()
        {
            var dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                var databaseFile = dialog.FileName;
                var database = MemoryFileDatabase.Load(databaseFile);
                FileSystemViewModel.UpdateReferenceSystem(database);
            }
        }
    }

    public enum FilterMode
    {
        ShowAll,
        ShowDuplicates,
        ShowUnique,
    }

    public class FilterItem

    {
        public string Text { get; private set; }
        public object Value { get; private set; }

        public FilterItem(string text, object value)
        {
            Text = text;
            Value = value;
        }
    }
}
