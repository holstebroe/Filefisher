using System.IO;
using System.Text;

namespace FilefisherWpf.ViewModels
{
    public class TextFilePreview : IFilePreview
    {
        public TextFilePreview(string path)
        {
            Path = path;

            if (File.Exists(path))
            {
                var lineCount = 0;
                var maxLines = 20;
                var lines = new StringBuilder();
                using (var reader = File.OpenText(path))
                {
                    var line = reader.ReadLine();
                    while (line != null)
                    {
                        lineCount++;
                        if (lineCount <= maxLines)
                            lines.AppendLine(line);
                        line = reader.ReadLine();
                    }
                    Content = lines.ToString();
                }
            }
            else
            {
                Content = "Could not read";
            }
        }

        public string Path { get; }

        public string Content { get; }

    }
}