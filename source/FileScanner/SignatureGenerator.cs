using System;
using System.IO;
using System.Security.Cryptography;

namespace FileScanner
{
    /// <summary>
    /// Class for generating file or folder signatures.
    /// </summary>
    public class SignatureGenerator
    {
        private readonly StatSignatureGenerator statSignatureGenerator;

        public SignatureGenerator(IHashGenerator hashGenerator)
        {
            statSignatureGenerator = new StatSignatureGenerator(hashGenerator);
        }

        /// <summary>
        /// Updates file system properties for a file descriptor.
        /// If the file descriptor is a folder, some properties will be based on sub-folders and files.
        /// </summary>
        public void UpdateStats(FileDescriptor descriptor)
        {
            statSignatureGenerator.Generate(descriptor);
        }

        public void UpdateContentHash(FileDescriptor descriptor)
        {
            using (var stream = File.OpenRead(descriptor.Path))
            {
                SHA1 sha = new SHA1CryptoServiceProvider();
                descriptor.ContentHash = Convert.ToBase64String(sha.ComputeHash(stream));
            }
        }
    }
}
