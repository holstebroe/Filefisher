﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileScanner
{
    /// <summary>
    /// Dummy database that ignores all method calls.
    /// </summary>
    public class NullFileDatabase : IFileDatabase
    {
        public void UpdateDescriptor(FileDescriptor fileDescriptor)
        {
            // Does nothing
        }
    }
}
