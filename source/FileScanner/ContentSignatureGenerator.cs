using System.Collections.Generic;
using System.IO;

namespace FileScanner
{
    public class ContentSignatureGenerator : ISignatureGenerator
    {
        private readonly IHashGenerator hashGenerator;

        public ContentSignatureGenerator(IHashGenerator hashGenerator)
        {
            this.hashGenerator = hashGenerator;
        }

        public void UpdateFileSignature(FileDescriptor descriptor)
        {
            using (var stream = File.OpenRead(descriptor.Path))
            {
                descriptor.ContentHash = hashGenerator.Generate(stream);
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
    }
}
