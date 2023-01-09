namespace FilefisherWpf.ViewModels
{
    public class ShowAllFileDescriptorFilter : IFileDescriptorFilter
    {
        public bool Pass(FileDescriptorViewModel fileDescriptor)
        {
            return true;
        }
    }
}