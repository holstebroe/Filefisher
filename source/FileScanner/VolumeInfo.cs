using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace FileScanner
{
    public class VolumeInfo
    {
        public VolumeInfo(string path)
        {
            var driveLetter = Path.GetPathRoot(path);
            var volumeName = new StringBuilder(256);
            var fileSystemName = new StringBuilder(256);

            if (GetVolumeInformation(
                driveLetter,
                volumeName,
                (uint) volumeName.Capacity,
                out var serialNumber,
                out _,
                out var fileSystemFlags,
                fileSystemName,
                (uint) fileSystemName.Capacity))
            {
                VolumeName = volumeName.ToString();
                SerialNumber = serialNumber.ToString();
                FileSystem = fileSystemName.ToString();
                Flags = "&&H" + fileSystemFlags.ToString("x");
            }

            var driveInfo = DriveInfo.GetDrives().FirstOrDefault(x => x.Name == driveLetter);
            if (driveInfo != null)
            {
                DriveType = driveInfo.DriveType.ToString();
                TotalSize = driveInfo.TotalSize;
                TotalFreeSpace = driveInfo.TotalFreeSpace;
            }
        }

        //private void Query(string path)
        //{
        //    ManagementObjectSearcher ms = new ManagementObjectSearcher("Select * from Win32_Volume");
        //    foreach (ManagementObject mo in ms.Get())
        //    {
        //        var guid = mo["DeviceID"].ToString();

        //        if (guid == myGuid)
        //            return mo["DriveLetter"];
        //    }
        //}


        public string Flags { get; set; }

        public string FileSystem { get; set; }

        public string SerialNumber { get; set; }

        public string VolumeName { get; set; }

        public string DriveType { get; set; }

        public long TotalSize { get; set; }

        public long TotalFreeSpace { get; set; }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern bool GetVolumeInformation(string volume, StringBuilder volumeName,
            uint volumeNameSize, out uint serialNumber, out uint serialNumberLength,
            out uint flags, StringBuilder fs, uint fsSize);
    }
}