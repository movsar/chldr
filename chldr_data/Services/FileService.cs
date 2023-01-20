using chldr_dataaccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace chldr_dataaccess.Services.PartialMethods
{
    // ! MAIN
    public class FileService
    {

        #region Fields
        public const string DatabaseName = "database.realm";
        public const string DataDirName = "data";
        public static string? AppDirectory;
        public static string? AppDataDirectory;
        public static string? DatabasePath;
        #endregion

        public FileService()
        {
            AppDirectory = AppContext.BaseDirectory;
            AppDataDirectory = Path.Combine(AppDirectory, DataDirName);
            DatabasePath = Path.Combine(AppDirectory, "Assets", DatabaseName);

            if (!Directory.Exists(AppDataDirectory))
            {
                Directory.CreateDirectory(AppDataDirectory);
            }
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
