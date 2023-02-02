using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace chldr_utils.Services
{
    // ! MAIN
    public class FileService
    {

        #region Fields
        private readonly string DataDirName = "data";
        public readonly string CompressedDatabaseFileName = "synced.base.dat";
        public readonly string DatabaseFileName = "synced.base.dbx";

        public string AppBaseDirectory;
        public string AppDataDirectory;
        public string CompressedDatabaseFilePath;
        #endregion

        public FileService(string basePath)
        {
            AppBaseDirectory = basePath;
            AppDataDirectory = Path.Combine(AppBaseDirectory, DataDirName);
            CompressedDatabaseFilePath = Path.Combine(AppDataDirectory, CompressedDatabaseFileName);

            if (!Directory.Exists(AppDataDirectory))
            {
                Directory.CreateDirectory(AppDataDirectory);
            }
        }

        public string GetUserDatabaseName(string id)
        {
            return $"{id.Substring(4, 4)}.dbx";
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
