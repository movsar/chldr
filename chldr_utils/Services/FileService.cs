using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace chldr_utils.Services
{
    public class FileService
    {

        #region Fields
        private const string DataDirName = "data";
        private const string OfflineDatabaseFileName = "offline.datx";

        public string AppBaseDirectory;
        public string AppDataDirectory => Path.Combine(AppBaseDirectory, DataDirName);
        public string OfflineDatabaseFilePath => Path.Combine(AppDataDirectory, OfflineDatabaseFileName);
        #endregion

        public FileService(string basePath)
        {
            AppBaseDirectory = basePath;

            if (!Directory.Exists(AppDataDirectory))
            {
                Directory.CreateDirectory(AppDataDirectory);
            }
        }

        async Task PrepareDatabaseFile_Android()
        {
            //AppDirectory = FileSystem.Current.AppDataDirectory;
            //using (FileStream writeStream = new FileStream(DatabasePath, FileMode.Create, FileAccess.Write))
            //{
            //    // Gets the realm database file from assets 
            //    using var dbFileStream = await FileSystem.OpenAppPackageFileAsync(DatabaseName);
            //    // Copies to the device
            //    dbFileStream.CopyTo(writeStream);
            //}
        }

    }
}
