using chldr_data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace chldr_data.Services.PartialMethods
{
    public partial class FileService
    {
        public static string AppDataDirectory = Path.Combine(AppContext.BaseDirectory, "Data");
        public const string DatabaseName = "database.realm";
        partial void PrepareDatabaseFile();

        public void PrepareDatabase()
        {
            PrepareDatabaseFile();
        }
    }
}
