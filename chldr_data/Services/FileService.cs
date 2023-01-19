using chldr_data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace chldr_data.Services.PartialMethods
{
    // ! MAIN
    public partial class FileService
    {

        #region Fields
        public const string DatabaseName = "database.realm";
        public const string DataDirName = "data";
        public static string AppDirectory;
        public static string AppDataDirectory;
        public static string DatabasePath;
        #endregion

        partial void PrepareDatabaseFile();
        public void PrepareDatabase()
        {
            var isWeb = Environment.StackTrace.Contains("chldr_server.Pages.Pages__Host.ExecuteAsync()");
            if (isWeb)
            {
                PrepareDatabaseFile_Web();
            }
            else
            {
                PrepareDatabaseFile();
            }
        }

        public void PrepareDatabaseFile_Web()
        {
            AppDirectory = AppContext.BaseDirectory;
            AppDataDirectory = Path.Combine(AppDirectory, DataDirName);
            DatabasePath = Path.Combine(AppDataDirectory, DatabaseName);

            if (!Directory.Exists(AppDataDirectory))
            {
                Directory.CreateDirectory(AppDataDirectory);
            }
        }
    }
}
