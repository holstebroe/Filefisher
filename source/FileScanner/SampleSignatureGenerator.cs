using System.Collections.Generic;
using System.IO;

namespace FileScanner
{
    /// <summary>
    /// Signature generator that calculates a hash key from sampled data in a file.
    /// Calculating a content hash based on samples will be much faster for large files.
    /// </summary>
    public class SampleSignatureGenerator : ISignatureGenerator
    {
        private const int SampleParts = 3;
        private const int SamplePartLength = 1024;

        private readonly IHashGenerator hashGenerator;

        public SampleSignatureGenerator(IHashGenerator hashGenerator)
        {
            this.hashGenerator = hashGenerator;
        }

        public void UpdateFileSignature(FileDescriptor descriptor)
        {
            using (var stream = File.OpenRead(descriptor.Path))
            {
                var sample = LoadSample(stream);
                descriptor.ContentHash = hashGenerator.Generate(sample);
            }
        }

        public void UpdateFolderSignature(FileDescriptor descriptor)
        {
            var allChildBytes = new List<byte>();
            if (descriptor.Children == null) return;
            foreach (var childDescriptor in descriptor.Children)
            {
                // TODO: Log warning if ContentHash is null. The hash may be null if access to the file/folder was denied.
                if (childDescriptor.ContentHash != null)
                {
                    allChildBytes.AddRange(childDescriptor.ContentHash);
                }
            }
            descriptor.ContentHash = hashGenerator.Generate(allChildBytes.ToArray());
        }

        private byte[] LoadSample(Stream stream)
        {
            var fileLength = stream.Length;
            var sampleLength = SampleParts * SamplePartLength;
            if (fileLength < sampleLength)
            {
                using (var memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);
                    return memoryStream.ToArray();
                }
            }
            var sample = new byte[sampleLength];

            for (int i = 0; i < SampleParts; i++)
            {
                stream.Seek(i * (fileLength - SamplePartLength) / (SampleParts - 1), SeekOrigin.Begin);
                stream.Read(sample, i * SamplePartLength, SamplePartLength);
            }
            return sample;
        }

    }
}
