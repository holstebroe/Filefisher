namespace FilefisherWpf.ViewModels
{
    public class ShowUniqueFilter : IFileDescriptorFilter
    {
        public bool Pass(FileDescriptorViewModel fileDescriptor)
        {
            return fileDescriptor.GetReferenceCount() == 0;
        }
    }
}