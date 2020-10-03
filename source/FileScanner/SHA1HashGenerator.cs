using System.IO;
using System.Security.Cryptography;

namespace FileScanner
{
    /// <summary>
    ///     Hash key generator that uses the SHA1 algorithm to generate a 160 bit hash key.
    ///     SHA1 is the algorithm used by Git.
    /// </summary>
    public class SHA1HashGenerator : IHashGenerator
    {
        private readonly SHA1 shaProvider = new SHA1CryptoServiceProvider();

        /// <summary>
        ///     Generate SHA1 hash key from bytes.
        /// </summary>
        public byte[] Generate(byte[] bytes)
        {
            return shaProvider.ComputeHash(bytes);
        }

        /// <summary>
        ///     Generate SHA1 hash key from stream of bytes.
        /// </summary>
        public byte[] Generate(Stream stream)
        {
            return shaProvider.ComputeHash(stream);
        }
    }
}