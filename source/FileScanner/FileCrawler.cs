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
            var basePath = baseDirectoryInfo.Parent.FullName;
            if (basePath != Path.GetPathRoot(baseDirectory))
                basePath += Path.DirectorySeparatorChar;
            return ScanDirectory(basePath, baseDirectoryInfo);            
        }

        private FileDescriptor ScanDirectory(string basePath, DirectoryInfo directoryInfo)
        {
            var directoryDescriptor = new FileDescriptor(basePath, directoryInfo.FullName);

            var descriptors = ScanFiles(basePath, directoryInfo);
            var subDirectories = directoryInfo.EnumerateDirectories("*.*", SearchOption.TopDirectoryOnly);
            foreach (var subDirectoryInfo in subDirectories)
            {
                var subDirectoryDescriptor = ScanDirectory(basePath, subDirectoryInfo);
                descriptors.Add(subDirectoryDescriptor);
            }
            directoryDescriptor.Children = descriptors;
            signatureGenerator.UpdateStats(directoryDescriptor);
            fileDatabase.UpdateDescriptor(directoryDescriptor);
            return directoryDescriptor;
        }

        private List<FileDescriptor> ScanFiles(string basePath, DirectoryInfo directoryInfo)
        {
            var descriptors = new List<FileDescriptor>();
            var files = directoryInfo.EnumerateFiles("*.*", SearchOption.TopDirectoryOnly);
            foreach (var fileInfo in files)
            {
                var descriptor = new FileDescriptor(basePath, fileInfo.FullName);
                if (!File.Exists(descriptor.FullPath))
                {
                    Console.WriteLine("Could not open {0}", descriptor.FullPath);
                    continue;
                }
                signatureGenerator.UpdateStats(descriptor);
                fileDatabase.UpdateDescriptor(descriptor);
                descriptors.Add(descriptor);
            }
            return descriptors;
        }
    }
}
