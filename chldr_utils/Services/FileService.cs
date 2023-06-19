using Microsoft.AspNetCore.Http;

namespace chldr_utils.Services
{
    public class FileService
    {

        #region Fields
        private const string DataDirName = "data";
        private const string OfflineDatabaseFileName = "local.datx";

        public static string AppBaseDirectory;
        public static string AppDataDirectory => Path.Combine(AppBaseDirectory, DataDirName);
        public static string OfflineDatabaseFilePath => Path.Combine(AppBaseDirectory, OfflineDatabaseFileName);
        public static string EntrySoundsDirectory => Path.Combine(AppDataDirectory, "sounds");
        #endregion
        static FileService()
        {
            AppBaseDirectory = AppContext.BaseDirectory;
        }

        public FileService(string basePath)
        {
            AppBaseDirectory = basePath;

            if (!Directory.Exists(EntrySoundsDirectory))
            {
                Directory.CreateDirectory(EntrySoundsDirectory);
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

        public async Task SaveSoundAsync(IFormFile contents, string fileName)
        {
            var filePath = Path.Combine(EntrySoundsDirectory, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await contents.CopyToAsync(stream);
            }
        }

        public static void AddEntrySound(string fileName, string b64data)
        {
            var filePath = Path.Combine(EntrySoundsDirectory, fileName);
            File.WriteAllText(filePath, b64data);
        }

        public static void DeleteEntrySound(string fileName)
        {
            var filePath = Path.Combine(EntrySoundsDirectory, fileName);
            File.Delete(filePath);
        }
    }
}
