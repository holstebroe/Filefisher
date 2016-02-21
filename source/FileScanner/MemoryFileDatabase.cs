using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;

namespace FileScanner
{
    /// <summary>
    /// File database that will keep the entire database in memory.
    /// </summary>
    [Serializable]
    public class MemoryFileDatabase : IFileDatabase
    {
        private const string DefaultFileExtension = ".fdb";
        private const string ApplicationDataFolderName = "Filefisher";
        private readonly Dictionary<string, FileDescriptor> descriptorMap = new Dictionary<string, FileDescriptor>();

        public MemoryFileDatabase()
        {
            
        }
        public MemoryFileDatabase(FileDescriptor rootDescriptor)
        {
            UpdateDescriptor(rootDescriptor);
            RootDescriptor = rootDescriptor;
            AddChildren(rootDescriptor);
        }

        private void AddChildren(FileDescriptor descriptor)
        {
            foreach (var child in descriptor.Children)
            {
                UpdateDescriptor(child);
                AddChildren(child);
            }
        }


        public FileDescriptor RootDescriptor { get; private set; }

        public RootInfo RootInfo { get; set; }

        public void UpdateDescriptor(FileDescriptor fileDescriptor)
        {
            descriptorMap[fileDescriptor.Path] = fileDescriptor;
            if (fileDescriptor.IsRoot)
            {
                if (RootDescriptor != null && fileDescriptor.Path != RootDescriptor.Path)
                    throw new InvalidOperationException(
                        $"Cannot assign {fileDescriptor.Path} as root descriptor. Root descriptor has already been assigned to {RootDescriptor.Path}");
                RootDescriptor = fileDescriptor;
            }
        }

        public IEnumerable<FileDescriptor> GetAll()
        {
            return GetAllDescriptors();
            //return GetDeep(RootDescriptor);
        }

        public FileDescriptor GetRoot()
        {
            return RootDescriptor;
        }

        public IEnumerable<FileDescriptor> GetDeep(FileDescriptor descriptor)
        {
            yield return descriptor;
            foreach (var child in descriptor.Children)
            {
                var childDescriptors = GetDeep(child);
                foreach (var childDescriptor in childDescriptors)
                {
                    yield return childDescriptor;
                }
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
        /// Returns default path name of this database based on the root descriptor.
        /// </summary>
        public string GetDefaultDatabasePath()
        {
            var rootDescriptor = RootDescriptor;
            var applicationDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            var programDataFolder = Path.Combine(applicationDataFolder, ApplicationDataFolderName);
            if (!Directory.Exists(programDataFolder))
                Directory.CreateDirectory(programDataFolder);
            var databaseFileName = RootInfo.GenerateDatabaseFileName() + DefaultFileExtension;
            var databasePath = Path.Combine(programDataFolder, databaseFileName);
            return databasePath;
        }

        /// <summary>
        /// Saves database to default database file name. <seealso cref="GetDefaultDatabasePath"/>.
        /// </summary>
        public void SaveDefault()
        {
            Save(GetDefaultDatabasePath());
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
            Console.WriteLine($"Saved database to {fileName}");
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
