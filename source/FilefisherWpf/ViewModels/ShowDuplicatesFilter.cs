using System.Linq;

namespace FilefisherWpf.ViewModels
{
    public class ShowDuplicatesFilter : IFileDescriptorFilter
    {
        public bool Pass(FileDescriptorViewModel fileDescriptor)
        {
            if (fileDescriptor is FolderViewModel folderViewModel)
                return folderViewModel.Children.Any(Pass);
            return fileDescriptor.GetReferenceCount() > 0;
        }
    }
}