using Microsoft.AspNetCore.Http;
using Xamarin.Essentials;

namespace chldr_utils.Services
{
    public class FileService
    {

        #region Fields
        private const string DataDirName = "Data";
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
            //var a = FileSystem.AppDataDirectory;

            if (!Directory.Exists(EntrySoundsDirectory))
            {
                Directory.CreateDirectory(EntrySoundsDirectory);
            }
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
