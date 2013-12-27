using System;
using System.Collections.Generic;
using System.IO;

namespace FileScanner
{
    /// <summary>
    /// The file crawler will traverse a file system tree and register all entries in a file database.
    /// </summary>
    public class FileCrawler
    {
        private readonly IFileDatabase fileDatabase;
        private readonly SignatureGenerator signatureGenerator;

        public FileCrawler(IFileDatabase fileDatabase, SignatureGenerator signatureGenerator)
        {
            this.fileDatabase = fileDatabase;
            this.signatureGenerator = signatureGenerator;
        }

        public FileDescriptor ScanDirectory(string baseDirectory)
        {
            var baseDirectoryInfo = new DirectoryInfo(baseDirectory);
            var basePath = baseDirectoryInfo.Parent.FullName+ Path.DirectorySeparatorChar;
            return ScanDirectory(basePath, baseDirectoryInfo);            
        }

        private FileDescriptor ScanDirectory(string basePath, DirectoryInfo directoryInfo)
        {
            var directoryDescriptor = new FileDescriptor(directoryInfo.FullName.Substring(basePath.Length));
            fileDatabase.UpdateDescriptor(directoryDescriptor);

            var descriptors = ScanFiles(basePath, directoryInfo);
            var subDirectories = directoryInfo.EnumerateDirectories("*.*", SearchOption.TopDirectoryOnly);
            foreach (var subDirectoryInfo in subDirectories)
            {
                ScanDirectory(basePath, subDirectoryInfo);
                descriptors.Add(directoryDescriptor);
            }
            directoryDescriptor.Children = descriptors;
            return directoryDescriptor;
        }

        private List<FileDescriptor> ScanFiles(string basePath, DirectoryInfo directoryInfo)
        {
            var descriptors = new List<FileDescriptor>();
            var files = directoryInfo.EnumerateFiles("*.*", SearchOption.TopDirectoryOnly);
            foreach (var fileInfo in files)
            {
                var descriptor = new FileDescriptor(fileInfo.FullName.Substring(basePath.Length));
                signatureGenerator.UpdateStats(descriptor);
                fileDatabase.UpdateDescriptor(descriptor);
                descriptors.Add(descriptor);
            }
            return descriptors;
        }
    }
}
