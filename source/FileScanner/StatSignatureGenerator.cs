using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileScanner
{
    /// <summary>
    /// Signature generator that generates a signature based on file stats: File name, modify time and size
    /// </summary>
    public class StatSignatureGenerator
    {
        private readonly IHashGenerator hashGenerator;

        public StatSignatureGenerator(IHashGenerator hashGenerator)
        {
            this.hashGenerator = hashGenerator;
        }

        public void Generate(FileDescriptor fileDescriptor)
        {
            fileDescriptor.StatHash =
                fileDescriptor.IsFolder 
                    ? GenerateFolderSignature(fileDescriptor) 
                    : GenerateFileSignature(fileDescriptor);
        }

        /// <summary>
        /// The folder signature is based on the combined stat hash of all folder children.
        /// The signature of subfolders are included. This will make changes deep down the folder
        /// structure bubble up and change the signature of the base folder. This will allow us
        /// to detect deep differences in two folders even if they contain the same direct children.
        /// </summary>
        private byte[] GenerateFolderSignature(FileDescriptor fileDescriptor)
        {
            var allChildBytes = new List<byte>();
            foreach (var descriptor in fileDescriptor.Children)
            {
                allChildBytes.AddRange(descriptor.StatHash);
            }
            return hashGenerator.Generate(allChildBytes.ToArray());
        }

        /// <summary>
        /// File stat signature is based on file name, modify time and size.
        /// This allows us to detect changes with quite good confidence,
        /// but files will still match signature if a copy gets a new created time.
        /// </summary>
        private byte[] GenerateFileSignature(FileDescriptor fileDescriptor)
        {
            var nameBytes = Encoding.UTF8.GetBytes(fileDescriptor.Name);
            var modifyBytes = BitConverter.GetBytes(fileDescriptor.ModifyTime.Ticks);
            var sizeBytes = BitConverter.GetBytes(fileDescriptor.Size);
            var allBytes = nameBytes.Concat(modifyBytes).Concat(sizeBytes).ToArray();
            return hashGenerator.Generate(allBytes);
        }
    }
}
