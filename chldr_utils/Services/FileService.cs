using Microsoft.AspNetCore.Http;

namespace chldr_utils.Services
{
    public class FileService
    {

        #region Fields
        private const string DataDirName = "data";
        private const string OfflineDatabaseFileName = "local.datx";

        public string AppBaseDirectory;
        public string AppDataDirectory => Path.Combine(AppBaseDirectory, DataDirName);
        public string OfflineDatabaseFilePath => Path.Combine(AppBaseDirectory, OfflineDatabaseFileName);
        public string EntrySoundsDirectory => Path.Combine(AppDataDirectory, "sounds");
        #endregion

        public FileService() : this(AppContext.BaseDirectory) { }
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

        public void AddEntrySound(string fileName, string b64data)
        {
            var filePath = Path.Combine(EntrySoundsDirectory, fileName);
            File.WriteAllText(filePath, b64data);
        }

        public void DeleteEntrySound(string fileName)
        {
            var filePath = Path.Combine(EntrySoundsDirectory, fileName);
            File.Delete(filePath);
        }
    }
}
