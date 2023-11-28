﻿using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace chldr_utils.Services
{
    public class FileService
    {
        public const string DataArchiveName = "data.zip";

        #region Fields
        private const string DataDirName = "data";
        private const string OfflineDatabaseFileName = "local.datx";

        public string AppBaseDirectory;

        public string DataArchiveFilePath => Path.Combine(AppBaseDirectory, DataArchiveName);
        public string AppDataDirectory => Path.Combine(AppBaseDirectory, DataDirName);
        public string DatabaseFilePath => Path.Combine(AppDataDirectory, OfflineDatabaseFileName);
        public string EntrySoundsDirectory => Path.Combine(AppDataDirectory, "sounds");
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
