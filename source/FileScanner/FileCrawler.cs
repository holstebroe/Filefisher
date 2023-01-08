using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FileScanner
{
    /// <summary>
    ///     The file crawler will traverse a file system tree and register all entries in a file database.
    /// </summary>
    public class FileCrawler
    {
        private readonly IFileDescriptorProvider descriptorProvider;
        private readonly IFileDatabase fileDatabase;
        private readonly IProgressTracker progressTracker;
        private readonly ISignatureGenerator signatureGenerator;

        public FileCrawler(IFileDatabase fileDatabase, IFileDescriptorProvider descriptorProvider,
            ISignatureGenerator signatureGenerator, IProgressTracker progressTracker)
        {
            this.fileDatabase = fileDatabase;
            this.descriptorProvider = descriptorProvider;
            this.signatureGenerator = signatureGenerator;
            this.progressTracker = progressTracker;
        }

        public FileDescriptor ScanDirectory(string baseDirectory)
        {
            return ScanDirectoryAsync(baseDirectory).Result;
        }
        public async Task<FileDescriptor> ScanDirectoryAsync(string baseDirectory)
        {
            var fullBasePath = Path.GetFullPath(baseDirectory);
            var baseDescriptor = new FileDescriptor(baseDirectory, fullBasePath) {IsRoot = true, IsFolder = true};
            await ScanDirectoryAsync(baseDescriptor);
            return baseDescriptor;
        }

        public void ScanDirectory(FileDescriptor directoryDescriptor)
        {
            ScanDirectoryAsync(directoryDescriptor).Wait();
        }
        public async Task ScanDirectoryAsync(FileDescriptor directoryDescriptor)
        {
            progressTracker.Start();
            await Task.Run(() => ScanSubDirectoryAsync(directoryDescriptor));
            progressTracker.Stop();
        }

        private void ScanSubDirectoryAsync(FileDescriptor directoryDescriptor)
        {
            try
            {
                var subDescriptors = ScanFiles(directoryDescriptor);
                var subDirectories = descriptorProvider.GetDirectories(directoryDescriptor);
                foreach (var subDirectory in subDirectories)
                {
                    ScanSubDirectoryAsync(subDirectory);
                    subDescriptors.Add(subDirectory);
                }

                directoryDescriptor.Children = subDescriptors;
                signatureGenerator.UpdateFolderSignature(directoryDescriptor);
                UpdateFolderSize(directoryDescriptor);
                fileDatabase.UpdateDescriptor(directoryDescriptor);
            }
            catch (UnauthorizedAccessException)
            {
                // TODO log that we were not allowed to scan the directory
            }
        }

        private void UpdateFolderSize(FileDescriptor descriptor)
        {
            if (descriptor.Children == null || !descriptor.IsFolder) return;
            descriptor.Size = descriptor.Children.Select(x => x.Size).Sum();
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

        public static FileDescriptor StatScanFolder(MemoryFileDatabase database, string baseFolder, IProgressTracker progressTracker)
        {
            return StatScanFolderAwait(database, baseFolder, progressTracker).Result;
        }

        public static async Task<FileDescriptor> StatScanFolderAwait(MemoryFileDatabase database, string baseFolder,
            IProgressTracker progressTracker)
        {
            var volumeInfo = new VolumeInfo(baseFolder);
            Console.WriteLine($"Name = {volumeInfo.VolumeName}, Serial = {volumeInfo.SerialNumber}");
            database.RootInfo = new RootInfo
            {
                RootPath = baseFolder,
                VolumeId = volumeInfo.SerialNumber,
                VolumeLabel = volumeInfo.VolumeName,
                FileSystem = volumeInfo.FileSystem,
                DriveType = volumeInfo.DriveType,
                TotalSize = volumeInfo.TotalSize,
                TotalFreeSpace = volumeInfo.TotalFreeSpace
            };
            var signatureGenerator = new StatSignatureGenerator(new SHA1HashGenerator());
            var crawler = new FileCrawler(database, new SystemFileDescriptorProvider(), signatureGenerator,
                progressTracker);
            var scanTimer = Stopwatch.StartNew();
            var rootDescriptor = await crawler.ScanDirectoryAsync(baseFolder);
            scanTimer.Stop();
            var descriptorCount = database.GetAllDescriptors().Count();
            Console.WriteLine("Scanned {0} entries in {1}. {2} stat scans per second", descriptorCount,
                scanTimer.Elapsed,
                1000 * descriptorCount / scanTimer.ElapsedMilliseconds);
            return rootDescriptor;
        }

        public static async Task UpdateContentSignaturesAsync(MemoryFileDatabase database, FileDescriptor rootDescriptor,
            IProgressTracker progressTracker)
        {
            Console.WriteLine("Updating content signatures");
            var contentCrawler = new FileCrawler(new NullFileDatabase(), new RevisitDescriptorProvider(),
                new SampleSignatureGenerator(new SHA1HashGenerator()), progressTracker);
            var contentTimer = Stopwatch.StartNew();
            await contentCrawler.ScanDirectoryAsync(rootDescriptor);
            contentTimer.Stop();
            var descriptorCount = database.GetAllDescriptors().Count();
            //            PrintDescriptorTree(rootDescriptor, descriptor => descriptor.ContentHash);
            //            PrintDuplicates(database, descriptor => descriptor.ContentHash);
            Console.WriteLine("Calculated content signature for {0} entries in {1}. {2} files per second",
                descriptorCount,
                contentTimer.Elapsed, 1000 * descriptorCount / contentTimer.ElapsedMilliseconds);
        }
    }
}