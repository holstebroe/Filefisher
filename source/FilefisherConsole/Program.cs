using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLine;
using CommandLine.Text;
using FileScanner;
using FileScanner.Duplicates;

// Options examples
// -f C:\Temp\DCIM_import_done\Incoming --duplicateMode=Content --readcontent -d "c:\Temp\dcim.fdb" -p
// -f C:\Temp\MyTrack --duplicateMode=Content --readcontent -d "c:\Temp\mytrack.fdb" -p
// --duplicateMode=Content --fileduplicates -d "c:\Temp\dcim.fdb" -c "C:\ProgramData\Filefisher\3155494950_C__Users_Søren_Pictures.fdb"
// --duplicateMode=Content --fileduplicates -d "c:\Temp\dcim.fdb" -c "C:\Temp\mytrack.fdb"
// --readcontent -d "D:\Søren\Arkiv\hd_mamut\videos_mamut.fdb" -p -f "D:\Videos"
// --readcontent -d "D:\Søren\Arkiv\hd08\projects.fdb" -p -f "F:\projects"


namespace FilefisherConsole
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var options = new Options();
            if (!Parser.Default.ParseArguments(args, options)) Environment.Exit(1);

            var baseFolder = options.ScanFolder;
            var database = new MemoryFileDatabase();

            var progressTracker = options.ShowProgress
                ? (IProgressTracker) new ConsoleProgressTracker(database.GetAllDescriptors().Count())
                : new NullProgressTracker();


            if (File.Exists(options.DatabaseFile) && string.IsNullOrEmpty(baseFolder))
                database = MemoryFileDatabase.Load(options.DatabaseFile);

            if (!string.IsNullOrEmpty(baseFolder))
            {
                FileDescriptor rootDescriptor = null;
                if (Directory.Exists(baseFolder))
                {
                    var fullBaseFolder = Path.GetFullPath(baseFolder);

                    rootDescriptor = FileCrawler.StatScanFolder(database, fullBaseFolder, progressTracker);
                }
                else if (File.Exists(options.ScanFolder))
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

                // Save database before long running content update to allow the user to abort.
                if (!string.IsNullOrEmpty(options.DatabaseFile))
                    database.Save(options.DatabaseFile);

                if (options.ReadContent)
                {
                    progressTracker.Restart();
                    FileCrawler.UpdateContentSignaturesAsync(database, rootDescriptor, progressTracker).Wait();
                }

                if (!string.IsNullOrEmpty(options.DatabaseFile))
                    database.Save(options.DatabaseFile);
                if (options.Verbose) PrintDescriptorTree(rootDescriptor, descriptor => descriptor.StatHash);
            }

            var duplicateComparer = options.DuplicateMode == DuplicateMode.Stat
                ? (IDuplicateComparer) new StatDuplicateComparer()
                : new ContentDuplicateComparer();

            var databaseA = database;

            var databaseB = string.IsNullOrEmpty(options.CompareDatabaseFile)
                ? database
                : MemoryFileDatabase.Load(options.CompareDatabaseFile);

            ShowDuplicates(options, databaseA, databaseB, duplicateComparer);
            ShowUnique(options, databaseA, databaseB, duplicateComparer);
        }

        private static void ShowDuplicates(Options options, MemoryFileDatabase databaseA, MemoryFileDatabase databaseB,
            IDuplicateComparer duplicateComparer)
        {
            if (options.ShowFileDuplicates)
            {
                Console.WriteLine("FILE DUPLICATES");
                Console.WriteLine("---------------");
                Console.WriteLine($"Files in {databaseA.RootDescriptor.Path}: {databaseA.GetAllDescriptors()}");
                Console.WriteLine($"Files in {databaseB.RootDescriptor.Path}: {databaseB.GetAllDescriptors()}");
                var duplicateFinder = new FileDuplicateFinder(duplicateComparer);
                var duplicates = duplicateFinder.FindDuplicates(databaseA, databaseB);
                PrintDuplicates(duplicates);
            }

            if (options.ShowTopDuplicates)
            {
                Console.WriteLine("TOP DUPLICATES");
                Console.WriteLine("--------------");
                var duplicateFinder = new TopDescriptorDuplicateFinder(duplicateComparer);
                var duplicates = duplicateFinder.FindDuplicates(databaseA, databaseB);
                PrintDuplicates(duplicates);
            }
        }

        private static void ShowUnique(Options options, MemoryFileDatabase databaseA, MemoryFileDatabase databaseB,
            IDuplicateComparer duplicateComparer)
        {
            if (options.ShowOnlyInDatabase)
            {
                Console.WriteLine("ONLY IN DATABASE");
                Console.WriteLine("---------------");
                Console.WriteLine($"Files in {databaseA.RootDescriptor.Path}: {databaseA.GetAllDescriptors()}");
                Console.WriteLine($"Files in {databaseB.RootDescriptor.Path}: {databaseB.GetAllDescriptors()}");
                var duplicateFinder = new FileDuplicateFinder(duplicateComparer);
                var uniqInA = duplicateFinder.FindUniqueA(databaseA, databaseB);
                foreach (var fileDescriptor in uniqInA) PrintDescriptor(fileDescriptor);
            }
        }

        private static void PrintDescriptor(FileDescriptor fileDescriptor)
        {
            Console.WriteLine($"{fileDescriptor.Path} {fileDescriptor.FormatSize()}");
        }

        private static void PrintDuplicates(IEnumerable<Duplicate> duplicates)
        {
            var duplicateSetIndex = 1;
            foreach (var duplicate in duplicates.OrderBy(x => x.Descriptors.First().FullPath))
            {
                var duplicateIndex = 1;
                foreach (var fileDescriptor in duplicate.Descriptors)
                {
                    Console.WriteLine(
                        $"[{duplicateSetIndex:D5}:{duplicateIndex}] {fileDescriptor.Path} {fileDescriptor.FormatSize()}");
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

        private static void PrintDescriptorTree(FileDescriptor rootDescriptor,
            Func<FileDescriptor, byte[]> signatureFunc, string indent = "")
        {
            var descriptor = rootDescriptor;
            var hashSignature = signatureFunc(descriptor);
            Console.WriteLine("{0}{1} {2}", indent, descriptor.Name,
                hashSignature != null ? Convert.ToBase64String(hashSignature) : "<null>");
            if (descriptor.Children != null)
                foreach (var child in descriptor.Children)
                    PrintDescriptorTree(child, signatureFunc, indent + "  ");
        }

        private static void ShowUsage()
        {
            Console.WriteLine("Filefisher <folder>");
        }
    }

    // ReSharper disable StringLiteralTypo
    internal class Options
    {
        [Option('f', "folder", Required = false, HelpText = "Folder to scan.")]
        public string ScanFolder { get; set; }

        [Option("readcontent", Required = false, HelpText = "Read content hash for each file")]
        public bool ReadContent { get; set; }

        [Option('d', "database", Required = false, HelpText = "Scan database file")]
        public string DatabaseFile { get; set; }

        [Option('c', "comparedatabase", Required = false, HelpText = "Scan comparison database file")]
        public string CompareDatabaseFile { get; set; }

        [Option('v', "verbose", HelpText = "Print details during execution.")]
        public bool Verbose { get; set; }

        [Option('p', "progress", HelpText = "Show scan progress.")]
        public bool ShowProgress { get; set; }

        [Option("duplicateMode", HelpText = "Comparison mode when searching for duplicates [Stat, Content].",
            DefaultValue = DuplicateMode.Stat)]
        public DuplicateMode DuplicateMode { get; set; }

        [Option("fileduplicates", HelpText = "Show all file duplicates.")]
        public bool ShowFileDuplicates { get; set; }

        [Option("onlyindatabase", HelpText = "Show all files only in database, but not in compare database.")]
        public bool ShowOnlyInDatabase { get; set; }

        [Option("topduplicates", HelpText = "Show top most path duplicates.")]
        public bool ShowTopDuplicates { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, current => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
    // ReSharper restore StringLiteralTypo

    public enum DuplicateMode
    {
        Stat,
        Content
    }
}