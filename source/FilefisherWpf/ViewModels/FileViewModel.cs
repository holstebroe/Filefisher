using System.Linq;
using FileScanner;
using Microsoft.VisualBasic.FileIO;

namespace FilefisherWpf.ViewModels
{
    public class FileViewModel : FileDescriptorViewModel
    {

        public FileViewModel(FileDescriptor descriptor, DescriptorLookup lookup, FolderViewModel parent) : base(
            descriptor, lookup, parent)
        {
        }


        public void Delete()
        {
            FileSystem.DeleteFile(Path, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
            Descriptor.Size = -1;

            var duplicates = Duplicates.Select(x => x.Descriptor).ToList();
            var root = Parent;
            if (root != null)
                while (root.Parent != null)
                    root = root.Parent;

            Parent.Remove(this);
            foreach (var duplicate in duplicates) FindAndUpdateDescriptor(root, duplicate);
        }

        private void FindAndUpdateDescriptor(FolderViewModel node, FileDescriptor fileDescriptor)
        {
            if (node == null) return;
            foreach (var child in node.Children)
            {
                if (child.Descriptor == fileDescriptor)
                {
                    child.Parent?.Update();
                    return;
                }

                if (child is FolderViewModel folderViewModel)
                    FindAndUpdateDescriptor(folderViewModel, fileDescriptor);
            }
        }
    }
}