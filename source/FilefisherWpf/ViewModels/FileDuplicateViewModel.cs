using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;
using FileScanner;

namespace FilefisherWpf.ViewModels
{
    public class FileDuplicateViewModel : ViewModelBase
    {
        private readonly IReadOnlyList<FileDescriptor> allDuplicates;

        public FileDuplicateViewModel(FileDescriptor descriptor, bool matchStat, bool matchContent,
            IReadOnlyList<FileDescriptor> allDuplicates)
        {
            this.allDuplicates = allDuplicates;
            Descriptor = descriptor;
            MatchStat = matchStat;
            MatchContent = matchContent;
            DeleteCommand = new RelayCommand(DeleteFile, CanDeleteFile);
        }

        public ICommand DeleteCommand { get; }

        public FileDescriptor Descriptor { get; }
        public bool MatchStat { get; }
        public bool MatchContent { get; }
        public bool Exists => File.Exists(Descriptor.FullPath);

        private bool CanDeleteFile()
        {
            var existingDuplicates = allDuplicates.Count(x => File.Exists(x.FullPath));
            return File.Exists(Descriptor.FullPath) && existingDuplicates > 1;
        }

        private void DeleteFile()
        {
            File.Delete(Descriptor.FullPath);
            NotifyPropertyChanged(nameof(Exists));
        }
    }
}