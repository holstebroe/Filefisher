using System;
using System.Collections;
using System.IO;

namespace FileScannerTest
{
    class TestResources
    {
        public const string ResourcesPath = "Resources";
        public const string SubPath = ResourcesPath + @"\" + "Sub";
        public const string EinsteinJpegFileName = @"albert-einstein.jpg";
        public const string EinsteinJpegPath = ResourcesPath + @"\" + EinsteinJpegFileName;
        public const string TextFilePath = ResourcesPath + @"\TextFile.txt";
        public const string SubTextFilePath = SubPath + @"\TextFile.txt";

        public static readonly DateTime EinsteinJpegCreateTime = new DateTime(2013, 12, 24, 12, 34, 56, 123, DateTimeKind.Utc);
        public static readonly DateTime EinsteinJpegModifyTime = new DateTime(2013, 12, 25, 21, 28, 48, 222, DateTimeKind.Utc);
        public static readonly DateTime SubFolderCreateTime = new DateTime(2013, 1, 1, 1, 1, 1, 1, DateTimeKind.Utc);
        public static readonly DateTime SubFolderModifyTime = new DateTime(2013, 1, 1, 1, 1, 1, 2, DateTimeKind.Utc);

        public static string[] AllPaths
        {
            get
            {
                return new[]
                    {
                        ResourcesPath,
                        EinsteinJpegPath, 
                        TextFilePath,
                        SubPath,
                        SubTextFilePath
                    };
            }
        }

        public static void Initialize()
        {
            File.SetCreationTimeUtc(EinsteinJpegPath, EinsteinJpegCreateTime);
            File.SetLastWriteTimeUtc(EinsteinJpegPath, EinsteinJpegModifyTime);
            Directory.SetCreationTimeUtc(SubPath, SubFolderCreateTime);
            Directory.SetLastWriteTimeUtc(SubPath, SubFolderModifyTime);
        }
    }
}
