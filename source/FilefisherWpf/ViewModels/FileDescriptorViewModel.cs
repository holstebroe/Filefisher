using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media;
using FileScanner;

namespace FilefisherWpf.ViewModels
{
    public abstract class FileDescriptorViewModel : ViewModelBase
    {
        protected readonly DescriptorLookup DescriptorLookup;
        private FileDescriptor descriptor;
        private FileDuplicateViewModel selectedDuplicate;
        private bool isSelected;

        protected FileDescriptorViewModel(FileDescriptor descriptor, DescriptorLookup lookup, FolderViewModel parent)
        {
            this.descriptor = descriptor;
            DescriptorLookup = lookup;
            Parent = parent;
        }

        public FileDescriptor Descriptor
        {
            get => descriptor;
            set
            {
                if (Equals(descriptor, value)) return;
                descriptor = value;
                NotifyPropertyChanged(nameof(MultiLineDescription));
                OnPropertyChanged();
            }

        }

        public FolderViewModel Parent { get; }

        public string Name => Descriptor.Name;
        public string Path => Descriptor.FullPath;

        public long Size => Descriptor.Size;

        public DateTime CreateTime => Descriptor.CreateTime;

        public DateTime ModifyTime => Descriptor.ModifyTime;

        public bool Exists => File.Exists(Descriptor.FullPath);

        public IEnumerable<FileDuplicateViewModel> Duplicates
        {
            get
            {
                if (DescriptorLookup == null) yield break;
                var statDuplicates = DescriptorLookup.LookupByStat(Descriptor).DistinctBy(x => x.FullPath).ToList();
                var contentDuplicates =
                    DescriptorLookup.LookupByContent(Descriptor).DistinctBy(x => x.FullPath).ToList();
                var allDuplicates = statDuplicates.Concat(contentDuplicates).DistinctBy(x => x.FullPath);
                var statDuplicatePaths = new HashSet<string>(statDuplicates.Select(x => x.FullPath));
                var contentDuplicatePaths = new HashSet<string>(contentDuplicates.Select(x => x.FullPath));

                var nonZeroDuplicates = allDuplicates.Where(x => x.Size > 0).ToList();
                foreach (var fileDescriptor in nonZeroDuplicates)
                    yield return new FileDuplicateViewModel(fileDescriptor,
                        statDuplicatePaths.Contains(fileDescriptor.FullPath),
                        contentDuplicatePaths.Contains(fileDescriptor.FullPath),
                        nonZeroDuplicates);
            }
        }

        public FileDuplicateViewModel SelectedDuplicate
        {
            get => selectedDuplicate;
            set
            {
                if (Equals(selectedDuplicate, value)) return;
                selectedDuplicate = value;
//                Descriptor = selectedDuplicate?.Descriptor;
                OnPropertyChanged();
            }
        }


        public string MultiLineDescription => FormatDescription(Descriptor);

        public IFilePreview FilePreview
        {
            get
            {
                var ext = System.IO.Path.GetExtension(Descriptor.FullPath);
                if (ext == null) return null;
                ext = ext.ToLower();

                // Image
                if (ext == ".jpg" || ext == ".png" || ext == ".gif" || ext == ".bmp" || ext == ".jpeg")
                    return new ImageFilePreview(Descriptor.FullPath);

                // Video
                if (ext == ".mp4" || ext == ".wmv" || ext == ".mov" || ext == ".m2ts" || ext == ".mts")
                    return new MediaFilePreview(Descriptor.FullPath);

                // Sound
                if (ext == ".mp3" || ext == ".wav" || ext == ".ogg")
                    return new MediaFilePreview(Descriptor.FullPath);

                // Text
                if (ext == ".txt" || ext == ".ini" || ext == ".yaml" || ext == ".config" || ext == ".cs" || ext == ".py")
                    return new TextFilePreview(Descriptor.FullPath);
                return null;
            }
        }

        public Brush Color
        {
            get
            {
                if (DescriptorLookup == null) return null;
                var statMatch = DescriptorLookup.LookupByStat(Descriptor).ToList().Any();
                var contentMatch = DescriptorLookup.LookupByContent(Descriptor).ToList().Any();
                if (statMatch && contentMatch) return Brushes.Red;
                if (!statMatch && !contentMatch) return Brushes.Green;
                return Brushes.GreenYellow;
            }
        }

        public bool IsSelected
        {
            get => isSelected;
            set
            {
                if (value == isSelected) return;
                isSelected = value;
                OnPropertyChanged();
            }
        }
        private string FormatDescription(FileDescriptor fileDescriptor)
        {
            var builder = new StringBuilder();
            builder.AppendLine($"Full path: {fileDescriptor.FullPath}");
            builder.AppendLine($"Path: {fileDescriptor.Path}");
            builder.AppendLine($"Size: {fileDescriptor.FormatSize()}");
            builder.AppendLine($"Created: {fileDescriptor.CreateTime:g}");
            builder.AppendLine($"Modified: {fileDescriptor.ModifyTime:g}");
            builder.AppendLine($"Stat hash: {FormatHash(fileDescriptor.StatHash)}");
            builder.AppendLine($"Content hash: {FormatHash(fileDescriptor.ContentHash)}");
            if (DescriptorLookup != null)
            {
                builder.AppendLine("References by stat:");
                FormatReferenceDescriptors(
                    ExceptThis(DescriptorLookup.LookupByStat(fileDescriptor).DistinctBy(x => x.FullPath)).ToList(),
                    builder);

                builder.AppendLine("References by content:");
                FormatReferenceDescriptors(
                    ExceptThis(DescriptorLookup.LookupByContent(fileDescriptor).DistinctBy(x => x.FullPath)).ToList(),
                    builder);
            }

            return builder.ToString();
        }

        private IEnumerable<FileDescriptor> ExceptThis(IEnumerable<FileDescriptor> descriptors)
        {
            return descriptors.Where(other => other.FullPath != Descriptor.FullPath);
        }

        public int GetReferenceCount()
        {
            var references = DescriptorLookup.LookupByStat(Descriptor)
                .Concat(DescriptorLookup.LookupByContent(Descriptor))
                .Where(x => x.Size > 0)
                .Select(x => x.FullPath)
                .Distinct()
                .ToList();
            references.Remove(Descriptor.FullPath);
            return references.Count;
        }

        private static void FormatReferenceDescriptors(IReadOnlyCollection<FileDescriptor> referenceDescriptors,
            StringBuilder builder)
        {
            builder.AppendLine($"Matches: {referenceDescriptors.Count}");
            if (referenceDescriptors.Count <= 50)
                foreach (var referenceDescriptor in referenceDescriptors)
                    builder.AppendLine($"Path: {referenceDescriptor.Path}");
        }


        private string FormatHash(byte[] hashSignature)
        {
            return hashSignature != null ? Convert.ToBase64String(hashSignature) : "<null>";
        }
    }
}