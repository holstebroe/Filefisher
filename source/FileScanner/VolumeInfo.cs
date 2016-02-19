using System;
using System.Collections.Generic;
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
            var drive_letter = Path.GetPathRoot(path);
            uint serial_number = 0;
            uint max_component_length = 0;
            StringBuilder sb_volume_name = new StringBuilder(256);
            UInt32 file_system_flags = new UInt32();
            StringBuilder sb_file_system_name = new StringBuilder(256);

            if (GetVolumeInformation(drive_letter, sb_volume_name,
                (UInt32)sb_volume_name.Capacity, out serial_number, out max_component_length,
                out file_system_flags, sb_file_system_name, (UInt32)sb_file_system_name.Capacity))
            {
                VolumeName = sb_volume_name.ToString();
                SerialNumber = serial_number.ToString();
//                lblMaxComponentLength.Text = max_component_length.ToString();
                FileSystem = sb_file_system_name.ToString();
                Flags = "&&H" + file_system_flags.ToString("x");
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


        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern bool GetVolumeInformation(string Volume, StringBuilder VolumeName,
            uint VolumeNameSize, out uint SerialNumber, out uint SerialNumberLength,
            out uint flags, StringBuilder fs, uint fs_size);
    }
}
