using chldr_data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
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
            PrepareDatabaseFile();
        }
    }
}
