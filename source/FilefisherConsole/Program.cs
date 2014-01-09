using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using FileScanner;

namespace FilefisherConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                ShowUsage();
            }
            var baseFolder = args[0];
            if (!Directory.Exists(baseFolder))
            {
                Console.WriteLine("No such directory: {0}", baseFolder);
                ShowUsage();
            }
            var database = new MemoryFileDatabase();
            var signatureGenerator = new StatSignatureGenerator(new SHA1HashGenerator());
            var crawler = new FileCrawler(database, new SystemFileDescriptorProvider(), signatureGenerator);
            var scanTimer = Stopwatch.StartNew();
            var rootDescriptor = crawler.ScanDirectory(baseFolder);
            scanTimer.Stop();
            SaveDatabase(database);
            PrintDescriptorTree(rootDescriptor, descriptor => descriptor.StatHash);
            PrintDuplicates(database, descriptor => descriptor.StatHash);
            var descriptorCount = database.GetAllDescriptors().Count();
            Console.WriteLine("Scanned {0} entries in {1}. {2} stat scans per second", descriptorCount, scanTimer.Elapsed, 1000 * descriptorCount / scanTimer.ElapsedMilliseconds);

            UpdateContentSignatures(database, rootDescriptor);
        }

        private static void SaveDatabase(MemoryFileDatabase database)
        {
            var applicationDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            var programDataFolder = Path.Combine(applicationDataFolder, "Filefisher");
            if (!Directory.Exists(programDataFolder))
                Directory.CreateDirectory(programDataFolder);
            var rootDescriptor = database.RootDescriptor;
            var databaseFileName = rootDescriptor.Name + ".fdb";
            var databasePath = Path.Combine(programDataFolder, databaseFileName);
            database.Save(databasePath);
        }

        private static void UpdateContentSignatures(MemoryFileDatabase database, FileDescriptor rootDescriptor)
        {
            var contentCrawler = new FileCrawler(new NullFileDatabase(), new RevisitDescriptorProvider(),
                                                 new SampleSignatureGenerator(new SHA1HashGenerator()));
            var contentTimer = Stopwatch.StartNew();
            contentCrawler.ScanDirectory(rootDescriptor);
            SaveDatabase(database);
            contentTimer.Stop();
            var descriptorCount = database.GetAllDescriptors().Count();
            PrintDescriptorTree(rootDescriptor, descriptor => descriptor.ContentHash);
            PrintDuplicates(database, descriptor => descriptor.ContentHash);
            Console.WriteLine("Calculated content signature for {0} entries in {1}. {2} files per second", descriptorCount,
                              contentTimer.Elapsed, 1000 * descriptorCount / contentTimer.ElapsedMilliseconds);
        }

        private static void PrintDuplicates(MemoryFileDatabase database, Func<FileDescriptor, byte[]> signatureFunc)
        {
            var duplicatesByStat = database.GetAllDescriptors()
                .Where(x => signatureFunc(x) != null)
                .GroupBy(x => Convert.ToBase64String(signatureFunc(x)))
                .Where(x => x.Count() > 1);
            Console.WriteLine("DUPLICATES");
            Console.WriteLine("----------");
            foreach (var duplicateGroup in duplicatesByStat)
            {
                Console.WriteLine("Duplicated signature {0}", Convert.ToBase64String(signatureFunc(duplicateGroup.First())));
                int duplicateIndex = 1;
                foreach (var fileDescriptor in duplicateGroup)
                {
                    Console.WriteLine("[{0}] {1}", duplicateIndex, fileDescriptor.Path);
                    duplicateIndex++;
                }

            }
        }

        private static void PrintDescriptorTree(FileDescriptor rootDescriptor, Func<FileDescriptor, byte[]> signatureFunc, string indent = "")
        {
            var descriptor = rootDescriptor;
            var hashSignature = signatureFunc(descriptor);
            Console.WriteLine("{0}{1} {2}", indent, descriptor.Name, hashSignature != null ? Convert.ToBase64String(hashSignature) : "<null>");
            if (descriptor.Children != null)
                foreach (var child in descriptor.Children)
                {
                    PrintDescriptorTree(child, signatureFunc, indent + "  ");
                }
        }

        private static void ShowUsage()
        {
            Console.WriteLine("Filefisher <folder>");
        }
    }
}
