using System;
using System.IO;

namespace FileScanner
{

    public class FileCrawler
    {
        public void ScanDirectory(string baseDirectory)
        {
            var baseDirectoryInfo = new DirectoryInfo(baseDirectory);
            var directories = baseDirectoryInfo.EnumerateDirectories("*.*", SearchOption.AllDirectories);
            foreach (var directoryInfo in directories)
            {
                Console.WriteLine("Scanning {0}", directoryInfo.FullName);
                ScanFiles(directoryInfo);
            }
        }

        private void ScanFiles(DirectoryInfo directoryInfo)
        {
            var files = directoryInfo.EnumerateFiles("*.*", SearchOption.TopDirectoryOnly);
            foreach (var fileInfo in files)
            {
                Console.WriteLine("   Scanning {0}", fileInfo.Name);
            }
        }
    }
}
