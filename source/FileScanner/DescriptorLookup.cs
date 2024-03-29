﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace FileScanner
{
    public class DescriptorLookup
    {
        // Ignore content matches for file sizes lower than this threshold.
        private const long ContentFileSizeThreshold = 5;

        private readonly MultiValueDictionary<string, FileDescriptor> contentLookup =
            new MultiValueDictionary<string, FileDescriptor>();

        private readonly MultiValueDictionary<string, FileDescriptor> statLookup =
            new MultiValueDictionary<string, FileDescriptor>();

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
                    if (key != null)
                        contentLookup.Add(key, fileDescriptor);
                }
            }
        }

        private static string GetStatKeyOrNull(FileDescriptor fileDescriptor)
        {
            return fileDescriptor.StatHash == null ? null : Convert.ToBase64String(fileDescriptor.StatHash);
        }

        private string GetContentKeyOrNull(FileDescriptor fileDescriptor)
        {
            // Ignore tiny files.
            if (fileDescriptor.Size < ContentFileSizeThreshold) return null;
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
            if (key == null)
                return Enumerable.Empty<FileDescriptor>();
            return contentLookup.TryGetValue(key, out var descriptors)
                ? descriptors
                : Enumerable.Empty<FileDescriptor>();
        }
    }
}