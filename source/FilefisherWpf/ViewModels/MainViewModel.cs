using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        }

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

        public ICommand LoadFileSystemCommand { get; }

        public FileSystemViewModel FileSystemViewModel
        {
            get { return fileSystemViewModel; }
            set { fileSystemViewModel = value; OnPropertyChanged();}
        }
    }
}
