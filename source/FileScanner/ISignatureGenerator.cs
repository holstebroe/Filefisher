using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileScanner
{
    /// <summary>
    /// Interface for signature generators that updates descriptor signatures
    /// </summary>
    public interface ISignatureGenerator
    {
        /// <summary>
        /// Updates signature in file descriptor.
        /// </summary>
        void UpdateFileSignature(FileDescriptor descriptor);

        /// <summary>
        /// Updates signature in folder descriptor. 
        /// It should be assumed that all child signatures have already been visited.
        /// </summary>
        void UpdateFolderSignature(FileDescriptor descriptor);
    }
}
