using domain.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace domain.Services
{
    public class FileService : IFileService
    {
        #region Fields
        private const string DataDirName = "data";
        private const string OfflineDatabaseFileName = "local.datx";

        public string AppBaseDirectory;

        public string DataArchiveName => "data.zip";
        public string DataArchiveFilePath => Path.Combine(AppBaseDirectory, DataArchiveName);
        public string AppDataDirectory => Path.Combine(AppBaseDirectory, DataDirName);
        public string DatabaseFilePath => Path.Combine(AppDataDirectory, "database", OfflineDatabaseFileName);
        public string EntrySoundsDirectory => Path.Combine(AppDataDirectory, "sounds");
        public string AlphabetSoundsDirectory => Path.Combine(AppDataDirectory, "alpha");
        #endregion

        public FileService() : this(AppContext.BaseDirectory) { }
        public FileService(string basePath)
        {
            Debug.WriteLine($"basePath = {basePath}");
            AppBaseDirectory = basePath;

            // Create all directories
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
