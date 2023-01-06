using Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Data.Services.PartialMethods
{
    public partial class FileService
    {
        public static string AppDataDirectory = AppContext.BaseDirectory;
        public const string DatabaseName = "database.realm";
        partial void PrepareDatabaseFile();

        public void PrepareDatabase()
        {
            PrepareDatabaseFile();
        }
    }
}
