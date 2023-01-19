using chldr_data.Interfaces;

namespace chldr_data.Services.PartialMethods
{
    // ! ANDROID
    public partial class FileService
    {
        public FileService()
        {
            AppDirectory = FileSystem.Current.AppDataDirectory;
            AppDataDirectory = Path.Combine(AppDirectory, DataDirName);
            DatabasePath = Path.Combine(AppDataDirectory, DatabaseName);

            if (!Directory.Exists(AppDataDirectory))
            {
                Directory.CreateDirectory(AppDataDirectory);
            }
        }
        partial void PrepareDatabaseFile()
        {
            // Checks if the file already exists.
            if (File.Exists(DatabasePath))
            {
                return;
            }

            Task.WaitAll(CopyDatabaseFile());
        }

        async Task CopyDatabaseFile()
        {
            using (FileStream writeStream = new FileStream(DatabasePath, FileMode.Create, FileAccess.Write))
            {
                // Gets the realm database file from assets 
                using var dbFileStream = await FileSystem.OpenAppPackageFileAsync(DatabaseName);
                // Copies to the device
                dbFileStream.CopyTo(writeStream);
            }
        }
    }
}