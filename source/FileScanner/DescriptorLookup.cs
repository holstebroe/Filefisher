using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileScanner
{
    public class DescriptorLookup
    {
        private readonly MultiValueDictionary<string, FileDescriptor> contentLookup = new MultiValueDictionary<string, FileDescriptor>();
        private readonly MultiValueDictionary<string, FileDescriptor> statLookup = new MultiValueDictionary<string, FileDescriptor>();

        public void Update(IEnumerable<FileDescriptor> fileDescriptors)
        {
            foreach (var fileDescriptor in fileDescriptors)
            {
                var statKey = GetStatKeyOrNull(fileDescriptor);
                if (statKey != null)
                    statLookup.Add(statKey, fileDescriptor);

                if (fileDescriptor.ContentHash != null)
                {
                    var key = GetContentKeyOrNull(fileDescriptor);
                    contentLookup.Add(key, fileDescriptor);
                }
            }
        }

        private static string GetStatKeyOrNull(FileDescriptor fileDescriptor)
        {
            return fileDescriptor.StatHash == null ? null : Convert.ToBase64String(fileDescriptor.StatHash);
        }

        private static string GetContentKeyOrNull(FileDescriptor fileDescriptor)
        {
            return fileDescriptor.ContentHash == null ? null : Convert.ToBase64String(fileDescriptor.ContentHash);
        }

        public IEnumerable<FileDescriptor> LookupByStat(FileDescriptor descriptor)
        {
            var key = GetStatKeyOrNull(descriptor);
            return statLookup.TryGetValue(key, out var descriptors) ? descriptors : Enumerable.Empty<FileDescriptor>();
        }
        public IEnumerable<FileDescriptor> LookupByContent(FileDescriptor descriptor)
        {
            var key = GetContentKeyOrNull(descriptor);
            return contentLookup.TryGetValue(key, out var descriptors) ? descriptors : Enumerable.Empty<FileDescriptor>();
        }
    }
}
