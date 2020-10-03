using System;
using System.IO;
using System.Linq;

namespace FileScanner
{
    [Serializable]
    public class RootInfo
    {
        public string RootPath { get; set; }
        public string VolumeId { get; set; }
        public string VolumeLabel { get; set; }
        public string FileSystem { get; set; }
        public string DriveType { get; set; }
        public long TotalSize { get; set; }
        public long TotalFreeSpace { get; set; }

        public string GenerateDatabaseFileName()
        {
            var fileName = !string.IsNullOrWhiteSpace(VolumeLabel) ? VolumeLabel + "_" : "";
            fileName += VolumeId + "_" + RootPath;
            return MakeValidFileName(fileName);
        }

        private string MakeValidFileName(string fileName)
        {
            return string.Join("_",
                fileName.Split(Path.GetInvalidFileNameChars().Concat(Path.GetInvalidPathChars()).ToArray()));
            //string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string();
            //var r = new Regex($"[{Regex.Escape(regexSearch)}]");
            //return r.Replace(fileName, "");
        }
    }
}