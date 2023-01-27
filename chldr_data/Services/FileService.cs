using chldr_data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace chldr_data.Services
{
    // ! MAIN
    public class FileService
    {

        #region Fields
        public const string OriginalDatabaseFileName = "database.realm";
        public const string DatabaseSuffix = "real.dbf";

        private const string DataDirName = "data";
        public static string? AppBaseDirectory;
        public static string? AppDataDirectory;
        public static string OriginalDatabasePath;
        #endregion

        public FileService(string basePath)
        {
            AppBaseDirectory = basePath;
            AppDataDirectory = Path.Combine(AppBaseDirectory, DataDirName);
            OriginalDatabasePath = Path.Combine(AppDataDirectory, OriginalDatabaseFileName);

            if (!Directory.Exists(AppDataDirectory))
            {
                Directory.CreateDirectory(AppDataDirectory);
            }
        }

        internal static string GetDatabasePath(string id)
        {
            return Path.Combine(AppDataDirectory, $"{id.Substring(4, 4)}.{DatabaseSuffix}");
        }

        public void PrepareDatabase()
        {
            var isWeb = Environment.StackTrace.Contains("chldr_server.Pages.Pages__Host.ExecuteAsync()");
            if (isWeb)
            {
                PrepareDatabaseFile_Web();
            }
        }

        public void PrepareDatabaseFile_Web()
        {

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
