using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            var signatureGenerator = new SignatureGenerator(new SHA1HashGenerator());
            var crawler = new FileCrawler(database, signatureGenerator);
            var rootDescriptor = crawler.ScanDirectory(baseFolder);
            PrintDescriptorTree(rootDescriptor);
            PrintDuplicates(database);
        }

        private static void PrintDuplicates(MemoryFileDatabase database)
        {
            var duplicatesByStat = database.GetAllDescriptors().GroupBy(x => Convert.ToBase64String(x.StatHash)).Where(x => x.Count() > 1);
            Console.WriteLine("DUPLICATES");
            Console.WriteLine("----------");
            foreach (var duplicateGroup in duplicatesByStat)
            {
                Console.WriteLine("Duplicated stat signature {0}", Convert.ToBase64String(duplicateGroup.First().StatHash));
                int duplicateIndex = 1;
                foreach (var fileDescriptor in duplicateGroup)
                {
                    Console.WriteLine("[{0}] {1}", duplicateIndex, fileDescriptor.Path);
                    duplicateIndex++;
                }

            }
        }

        private static void PrintDescriptorTree(FileDescriptor rootDescriptor, string indent = "")
        {
            var descriptor = rootDescriptor;
            Console.WriteLine("{0}{1} {2}", indent, descriptor.Name, descriptor.StatHash != null ? Convert.ToBase64String(descriptor.StatHash) : "<null>");
            if (descriptor.Children != null)
                foreach (var child in descriptor.Children)
                {
                    PrintDescriptorTree(child, indent + "  ");
                }
        }

        private static void ShowUsage()
        {
            Console.WriteLine("Filefisher <folder>");
        }
    }
}
