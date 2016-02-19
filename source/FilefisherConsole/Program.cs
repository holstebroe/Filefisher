﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using CommandLine;
using CommandLine.Text;
using FileScanner;

namespace FilefisherConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = new Options();
            if (!Parser.Default.ParseArguments(args, options))
            {
                Environment.Exit(1);
            }

            var baseFolder = options.ScanFolder;
            if (!Directory.Exists(baseFolder))
            {
                Console.WriteLine("No such directory: {0}", baseFolder);
                ShowUsage();
            }
            var database = new MemoryFileDatabase();
            if (!string.IsNullOrEmpty(options.ScanFolder))
            {
                var rootDescriptor = StatScanFolder(database, baseFolder);
                if (options.SaveDatabase)
                    database.SaveDefault();
                if (options.Verbose)
                {
                    PrintDescriptorTree(rootDescriptor, descriptor => descriptor.StatHash);
                    PrintDuplicates(database, descriptor => descriptor.StatHash);
                }
                if (options.ReadContent)
                    UpdateContentSignatures(database, rootDescriptor);
            }            
        }

        private static FileDescriptor StatScanFolder(MemoryFileDatabase database, string baseFolder)
        {
            VolumeInfo volumeInfo = new VolumeInfo(baseFolder);
            Console.WriteLine($"Name = {volumeInfo.VolumeName}, Serial = {volumeInfo.SerialNumber}");
            var signatureGenerator = new StatSignatureGenerator(new SHA1HashGenerator());
            var crawler = new FileCrawler(database, new SystemFileDescriptorProvider(), signatureGenerator);
            var scanTimer = Stopwatch.StartNew();
            var rootDescriptor = crawler.ScanDirectory(baseFolder);
            scanTimer.Stop();
            var descriptorCount = database.GetAllDescriptors().Count();
            Console.WriteLine("Scanned {0} entries in {1}. {2} stat scans per second", descriptorCount, scanTimer.Elapsed,
                1000*descriptorCount/scanTimer.ElapsedMilliseconds);
            return rootDescriptor;
        }

        private static void UpdateContentSignatures(MemoryFileDatabase database, FileDescriptor rootDescriptor)
        {
            var contentCrawler = new FileCrawler(new NullFileDatabase(), new RevisitDescriptorProvider(),
                                                 new SampleSignatureGenerator(new SHA1HashGenerator()));
            var contentTimer = Stopwatch.StartNew();
            contentCrawler.ScanDirectory(rootDescriptor);
            database.SaveDefault();
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

    class Options
    {
        [Option('f', "folder", Required = true, HelpText = "Folder to scan.")]
        public string ScanFolder { get; set; }

        [Option('c', "content", Required = false, HelpText = "Read content hash for each file")]
        public bool ReadContent { get; set; }

        [Option('s', "save", Required = false, HelpText = "Save scan to database file")]
        public bool SaveDatabase { get; set; }

        [Option('v', null, HelpText = "Print details during execution.")]
        public bool Verbose { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
