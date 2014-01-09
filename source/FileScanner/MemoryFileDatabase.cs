﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace FileScanner
{
    /// <summary>
    /// File database that will keep the entire database in memory.
    /// </summary>
    [Serializable]
    public class MemoryFileDatabase : IFileDatabase
    {
        private readonly Dictionary<string, FileDescriptor> descriptorMap = new Dictionary<string, FileDescriptor>();

        public FileDescriptor RootDescriptor { get; private set; }

        public void UpdateDescriptor(FileDescriptor fileDescriptor)
        {
            descriptorMap[fileDescriptor.Path] = fileDescriptor;
            if (fileDescriptor.IsRoot)
            {
                if (RootDescriptor != null && fileDescriptor.Path != RootDescriptor.Path)
                    throw new InvalidOperationException(string.Format("Cannot assign {0} as root descriptor. Root descriptor has already been assigned to {1}", fileDescriptor.Path, RootDescriptor.Path));
                RootDescriptor = fileDescriptor;
            }
        }

        /// <summary>
        /// Returns all file descriptors in the database.
        /// </summary>
        public IEnumerable<FileDescriptor> GetAllDescriptors()
        {
            return descriptorMap.Values;
        }


        /// <summary>
        /// Saves database into specified filename.
        /// </summary>
        public void Save(string fileName)
        {
            using (var stream = new FileStream(fileName, FileMode.Create))
            {
                using (var zipStream = new GZipStream(stream, CompressionMode.Compress))
                {
                    var formatter = new BinaryFormatter();
                    formatter.Serialize(zipStream, this);
                }
            }
        }

        /// <summary>
        /// Loads database from file.
        /// </summary>
        public static MemoryFileDatabase Load(string fileName)
        {
            using (var stream = new FileStream(fileName, FileMode.Open))
            {
                using (var zipStream = new GZipStream(stream, CompressionMode.Decompress))
                {
                    var formatter = new BinaryFormatter();
                    return (MemoryFileDatabase) formatter.Deserialize(zipStream);
                }
            }
        }
    }
}
