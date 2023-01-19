using chldr_data.Interfaces;

namespace chldr_data.Services.PartialMethods
{
    // ! WINDOWS
    public partial class FileService
    {
        #region Constructors
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
        #endregion
    }
}