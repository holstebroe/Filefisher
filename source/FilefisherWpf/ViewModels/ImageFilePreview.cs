namespace FilefisherWpf.ViewModels
{
    public class ImageFilePreview : IFilePreview
    {
        public ImageFilePreview(string path)
        {
            Path = path;
        }

        public string Path { get; }
    }
}