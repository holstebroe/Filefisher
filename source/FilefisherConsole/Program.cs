﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CommandLine;
using CommandLine.Text;
using FileScanner;
using FileScanner.Duplicates;

namespace FilefisherConsole
{
    static class Program
    {
        static void Main(string[] args)
        {
            var options = new Options();
            if (!Parser.Default.ParseArguments(args, options))
            {
                Environment.Exit(1);
            }

            var baseFolder = options.ScanFolder;
            var database = new MemoryFileDatabase();
            if (!string.IsNullOrEmpty(options.ScanFolder))
            {
                FileDescriptor rootDescriptor = null;
                if (Directory.Exists(options.ScanFolder))
                {
                    rootDescriptor = StatScanFolder(database, baseFolder);
                }
                else
                if (File.Exists(options.ScanFolder))
                {
                    database = MemoryFileDatabase.Load(options.ScanFolder);
                    rootDescriptor = database.RootDescriptor;
                }
                else
                {
                    Console.WriteLine($"Could not open {options.ScanFolder}");
                    Console.WriteLine(options.GetUsage());
                    Environment.Exit(1);
                }
                if (options.ReadContent)
                    UpdateContentSignatures(database, rootDescriptor);
                if (options.SaveDatabase)
                    database.SaveDefault();
                if (options.Verbose)
                {
                    PrintDescriptorTree(rootDescriptor, descriptor => descriptor.StatHash);
                }

                var duplicateComparer = options.DuplicateMode == DuplicateMode.Stat
                    ? (IDuplicateComparer) new StatDuplicateComparer()
                    : new ContentDuplicateComparer();

                if (options.ShowFileDuplicates)
                {
                    Console.WriteLine("FILE DUPLICATES");
                    Console.WriteLine("---------------");
                    var duplicateFinder = new FileDuplicateFinder(duplicateComparer);
                    var duplicates = duplicateFinder.Find(database, database);
                    PrintDuplicates(duplicates);
                }
                if (options.ShowTopDuplicates)
                {
                    Console.WriteLine("TOP DUPLICATES");
                    Console.WriteLine("--------------");
                    var duplicateFinder = new TopDescriptorDuplicateFinder(duplicateComparer);
                    var duplicates = duplicateFinder.Find(database, database);
                    PrintDuplicates(duplicates);
                }
            }            
        }

        private static FileDescriptor StatScanFolder(MemoryFileDatabase database, string baseFolder)
        {
            var volumeInfo = new VolumeInfo(baseFolder);
            Console.WriteLine($"Name = {volumeInfo.VolumeName}, Serial = {volumeInfo.SerialNumber}");
            database.RootInfo = new RootInfo
            {
                RootPath = baseFolder,
                VolumeId = volumeInfo.SerialNumber,
                VolumeLabel = volumeInfo.VolumeName
            };
            var progressTracker = new ConsoleProgressTracker();
            var signatureGenerator = new StatSignatureGenerator(new SHA1HashGenerator());
            var crawler = new FileCrawler(database, new SystemFileDescriptorProvider(), signatureGenerator, progressTracker);
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
            Console.WriteLine("Updating content signatures");
            var progressTracker = new ConsoleProgressTracker(database.GetAllDescriptors().Count());
            var contentCrawler = new FileCrawler(new NullFileDatabase(), new RevisitDescriptorProvider(),
                                                 new SampleSignatureGenerator(new SHA1HashGenerator()), progressTracker);
            var contentTimer = Stopwatch.StartNew();
            contentCrawler.ScanDirectory(rootDescriptor);
            contentTimer.Stop();
            var descriptorCount = database.GetAllDescriptors().Count();
//            PrintDescriptorTree(rootDescriptor, descriptor => descriptor.ContentHash);
//            PrintDuplicates(database, descriptor => descriptor.ContentHash);
            Console.WriteLine("Calculated content signature for {0} entries in {1}. {2} files per second", descriptorCount,
                              contentTimer.Elapsed, 1000 * descriptorCount / contentTimer.ElapsedMilliseconds);
        }

        private static void PrintDuplicates(IEnumerable<Duplicate> duplicates)
        {
            int duplicateSetIndex = 1;
            foreach (var duplicate in duplicates.OrderBy(x => x.Descriptors.First().FullPath))
            {
                int duplicateIndex = 1;
                foreach (var fileDescriptor in duplicate.Descriptors)
                {
                    Console.WriteLine($"[{duplicateSetIndex:D5}:{duplicateIndex}] {fileDescriptor.Path} {fileDescriptor.FormatSize()}");
                    duplicateIndex++;
                }
                Console.WriteLine();
                duplicateSetIndex++;
            }

        }

        //private static void PrintDuplicates(MemoryFileDatabase database, Func<FileDescriptor, byte[]> signatureFunc)
        //{
        //    var duplicatesByStat = database.GetAllDescriptors()
        //        .Where(x => signatureFunc(x) != null)
        //        .GroupBy(x => Convert.ToBase64String(signatureFunc(x)))
        //        .Where(x => x.Count() > 1);
        //    Console.WriteLine("DUPLICATES");
        //    Console.WriteLine("----------");
        //    foreach (var duplicateGroup in duplicatesByStat)
        //    {
        //        Console.WriteLine("Duplicated signature {0}", Convert.ToBase64String(signatureFunc(duplicateGroup.First())));
        //        int duplicateIndex = 1;
        //        foreach (var fileDescriptor in duplicateGroup)
        //        {
        //            Console.WriteLine("[{0}] {1}", duplicateIndex, fileDescriptor.Path);
        //            duplicateIndex++;
        //        }

        //    }
        //}

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

        [Option("readcontent", Required = false, HelpText = "Read content hash for each file")]
        public bool ReadContent { get; set; }

        [Option('s', "save", Required = false, HelpText = "Save scan to database file")]
        public bool SaveDatabase { get; set; }

        [Option('v', "verbose", HelpText = "Print details during execution.")]
        public bool Verbose { get; set; }

        [Option("duplicateMode", HelpText = "Comparison mode when searching for duplicates [Stat, Content].", DefaultValue = DuplicateMode.Stat)]
        public DuplicateMode DuplicateMode { get; set; }

        [Option("fileduplicates", HelpText = "Show all file duplicates.")]
        public bool ShowFileDuplicates { get; set; }

        [Option("topduplicates", HelpText = "Show top most path duplicates.")]
        public bool ShowTopDuplicates { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }

    public enum DuplicateMode
    {
        Stat,
        Content
    }
}
