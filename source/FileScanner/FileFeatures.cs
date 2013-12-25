using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileScanner
{
    public class FileFeatures
    {
        public FileFeatures(string fileName)
        {
            FileName = fileName;
        }

        public string FileName { get; private set; }
    }
}
