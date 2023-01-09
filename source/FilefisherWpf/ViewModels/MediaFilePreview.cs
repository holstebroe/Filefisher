namespace FilefisherWpf.ViewModels
{
    public class MediaFilePreview : IFilePreview
    {
        public MediaFilePreview(string path)
        {
            Path = path;
        }

        public string Path { get; }
    }
}