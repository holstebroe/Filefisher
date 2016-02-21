using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileScanner
{
    /// <summary>
    /// The file crawler will traverse a file system tree and register all entries in a file database.
    /// </summary>
    public class FileCrawler
    {
        private readonly IFileDatabase fileDatabase;
        private readonly IFileDescriptorProvider descriptorProvider;
        private readonly ISignatureGenerator signatureGenerator;
        private readonly IProgressTracker progressTracker;

        public FileCrawler(IFileDatabase fileDatabase, IFileDescriptorProvider descriptorProvider, ISignatureGenerator signatureGenerator, IProgressTracker progressTracker)
        {
            this.fileDatabase = fileDatabase;
            this.descriptorProvider = descriptorProvider;
            this.signatureGenerator = signatureGenerator;
            this.progressTracker = progressTracker;
        }

        public FileDescriptor ScanDirectory(string baseDirectory)
        {
            var baseDescriptor = new FileDescriptor(baseDirectory) { IsRoot = true};
            ScanDirectory(baseDescriptor);
            return baseDescriptor;
        }

        public void ScanDirectory(FileDescriptor directoryDescriptor)
        {
            progressTracker.Start();
            ScanSubDirectory(directoryDescriptor);
            progressTracker.Stop();
        }

        private void ScanSubDirectory(FileDescriptor directoryDescriptor)
        {
            try
            {
                var subDescriptors = ScanFiles(directoryDescriptor);
                var subDirectories = descriptorProvider.GetDirectories(directoryDescriptor);
                foreach (var subDirectory in subDirectories)
                {
                    ScanSubDirectory(subDirectory);
                    subDescriptors.Add(subDirectory);
                }
                directoryDescriptor.Children = subDescriptors;
                signatureGenerator.UpdateFolderSignature(directoryDescriptor);
                fileDatabase.UpdateDescriptor(directoryDescriptor);
            }
            catch (UnauthorizedAccessException)
            {
                // TODO log that we were not allowed to scan the directory
            }
        }

        private List<FileDescriptor> ScanFiles(FileDescriptor directoryDescriptor)
        {
            var descriptors = descriptorProvider.GetFiles(directoryDescriptor).ToList();

            foreach (var descriptor in descriptors)
            {
                if (!File.Exists(descriptor.FullPath))
                {
                    Console.WriteLine("Could not open {0}", descriptor.FullPath);
                    continue;
                }
                signatureGenerator.UpdateFileSignature(descriptor);
                fileDatabase.UpdateDescriptor(descriptor);
                progressTracker.Increment(descriptor.FullPath);
            }
            return descriptors;
        }
    }
}
