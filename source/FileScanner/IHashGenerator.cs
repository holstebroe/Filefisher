using System.IO;
using FileScanner.JetBrains.Annotations;

namespace FileScanner
{
    /// <summary>
    /// Interface for hash-key generator algorithms.
    /// </summary>
    public interface IHashGenerator
    {
        /// <summary>
        /// Generate hash key from bytes.
        /// </summary>
        [NotNull]
        byte[] Generate([NotNull] byte[] bytes);

        /// <summary>
        /// Generate hash key from stream of bytes.
        /// </summary>
        [NotNull]
        byte[] Generate([NotNull] Stream stream);
    }
}
