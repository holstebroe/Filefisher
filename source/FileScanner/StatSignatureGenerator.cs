using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace FileScanner
{
    /// <summary>
    /// Signature generator that generates a signature based on file stats: File name, modify time and size
    /// </summary>
    public class StatSignatureGenerator
    {
        public byte[] Generate(FileDescriptor fileDescriptor)
        {
            var nameBytes = Encoding.UTF8.GetBytes(fileDescriptor.Name);
            var modifyBytes = BitConverter.GetBytes(fileDescriptor.ModifyTime.Ticks);
            var sizeBytes = BitConverter.GetBytes(fileDescriptor.Size);
            var allBytes = nameBytes.Concat(modifyBytes).Concat(sizeBytes).ToArray();
            SHA1 sha = new SHA1CryptoServiceProvider();
            return sha.ComputeHash(allBytes);
        }
    }
}
