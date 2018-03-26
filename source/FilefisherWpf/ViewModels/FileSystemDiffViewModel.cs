using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileScanner;

namespace FilefisherWpf.ViewModels
{
    public class FileSystemDiffViewModel : ViewModelBase
    {
        private readonly MemoryFileDatabase database;
        private readonly MemoryFileDatabase referenceDatabase;
        private FileDescriptorViewModel selectedDescriptor;

        public FileSystemDiffViewModel(MemoryFileDatabase database, MemoryFileDatabase referenceDatabase)
        {
            this.database = database;
            this.referenceDatabase = referenceDatabase;

            // Recalculate folder-sizes if zero
            database.RootDescriptor.IsFolder = true;
            database.RootDescriptor.UpdateFolderSize();

            referenceDatabase.RootDescriptor.IsFolder = true;
            referenceDatabase.RootDescriptor.UpdateFolderSize();
//            Folders = new[] { new FolderViewModel(database.RootDescriptor) };
        }

        public IEnumerable<FolderDiffViewModel> FolderDiffs { get; }

    }

    public class FolderDiffViewModel
    {
        public FolderDiffViewModel(FileDescriptor aDescriptor, FileDescriptor bDescriptor)
        {

        }
    }
}
