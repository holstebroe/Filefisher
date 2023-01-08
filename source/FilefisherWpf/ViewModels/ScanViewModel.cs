using System.IO;
using System.Threading.Tasks;
using FileScanner;
using Microsoft.Win32;
using System.Windows.Input;
using AsyncAwaitBestPractices.MVVM;
using WK.Libraries.BetterFolderBrowserNS.Helpers;

namespace FilefisherWpf.ViewModels
{
    public class ScanViewModel : ViewModelBase
    {
        private MemoryFileDatabase database;
        private string databasePath;
        private string folderPath;
        private string status;
        public ProgressTrackerViewModel ProgressTracker { get; }
        public bool DoUpdateSignatures { get; set; } = true;

        public ScanViewModel()
        {
            SelectDatabaseCommand = new RelayCommand(SelectDatabase);
            SelectFolderCommand = new RelayCommand(SelectFolder);
            RescanCommand = new AsyncCommand(Rescan);

            ProgressTracker = new ProgressTrackerViewModel();
        }

        public string Status
        {
            get => status;
            set
            {
                if (value == status) return;
                status = value;
                OnPropertyChanged();
            }
        }

        private bool CanRescan(object arg)
        {
            return !string.IsNullOrEmpty(databasePath) && !string.IsNullOrEmpty(folderPath);
        }

        private async Task Rescan()
        {
            if (!CanRescan(null)) return;
            Status = "Start stat scan";
            database = new MemoryFileDatabase();
            await FileCrawler.StatScanFolderAwait(database, folderPath, ProgressTracker);
            Status = "Saving stat scan";
            database.Save(databasePath);
            if (DoUpdateSignatures)
            {
                Status = "Starting signature scan";
                ProgressTracker.Restart();
                await FileCrawler.UpdateContentSignaturesAsync(database, database.RootDescriptor, ProgressTracker);
                database.Save(databasePath);
                Status = "Stopping signature scan";
            }
        }
        private async Task RescanAsync()
        {
            await TestUpdateStatus();
            await TestUpdateStatus();
        }

        private async Task TestUpdateStatus()
        {
            ProgressTracker.Restart();
            for (int i = 0; i < 10; i++)
            {
                ProgressTracker.Increment("Update " + i);
                await Task.Delay(300);
            }
        }

        private void SelectFolder()
        {
            var dialog = new BetterFolderBrowserDialog
            {
                Title = "Select root folder"
            };
            if (database != null && Directory.Exists(database.GetRoot().FullPath))
            {
                dialog.InitialDirectory = database.GetRoot().FullPath;
            }
            if (dialog.ShowDialog())
            {
                var folder = dialog.FileName;
                FolderPath = folder;
            }
        }

        private void SelectDatabase()
        {
            var dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                var databaseFile = dialog.FileName;
                DatabasePath = databaseFile;
                //database = MemoryFileDatabase.Load(databaseFile);
            }
        }

        public ICommand SelectDatabaseCommand { get; }

        public string DatabasePath
        {
            get => databasePath;
            set
            {
                if (value == databasePath) return;
                databasePath = value;
                OnPropertyChanged();
            }
        }

        public ICommand SelectFolderCommand { get; }

        public string FolderPath
        {
            get => folderPath;
            set
            {
                if (value == folderPath) return;
                folderPath = value;
                OnPropertyChanged();
            }
        }

        public IAsyncCommand RescanCommand { get; }
    }
}
