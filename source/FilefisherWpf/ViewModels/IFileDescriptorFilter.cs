namespace FilefisherWpf.ViewModels
{
    public interface IFileDescriptorFilter
    {
        bool Pass(FileDescriptorViewModel fileDescriptor);
    }
}