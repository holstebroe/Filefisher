using System.Windows.Input;
using FileScanner;
using Microsoft.Win32;

namespace FilefisherWpf.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private FileSystemViewModel fileSystemViewModel;

        public MainViewModel()
        {
            LoadFileSystemCommand = new RelayCommand(LoadFileSystem);
            LoadReferenceSystemCommand = new RelayCommand(LoadReferenceSystem);
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
            }
        }

        public FileSystemViewModel FileSystemViewModel
        {
            get { return fileSystemViewModel; }
            set { fileSystemViewModel = value; OnPropertyChanged();}
        }

        public ICommand LoadReferenceSystemCommand { get; }
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
}
