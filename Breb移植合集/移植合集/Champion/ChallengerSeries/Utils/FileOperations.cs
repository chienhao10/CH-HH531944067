using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Challenger_Series.Utils
{
    public static class FileOperations
    {
        /// <summary>
        /// Gets the application data directory.
        /// </summary>
        /// <value>
        /// The application data directory.
        /// </value>
        public static string AppDataDirectory
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                        "LS" + Environment.UserName.GetHashCode().ToString("X"));
            }
        }
    }
}
