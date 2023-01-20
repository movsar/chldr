using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_dataaccess.Interfaces
{
    public interface IFileService
    {
        static string AppDirectory { get; set; }
        static string AppDataDirectory { get; set; }
        static string DatabasePath { get; set; }
        void PrepareDatabaseFile();

    }
}
