using chldr_data.Interfaces;

namespace chldr_data.Services.PartialMethods
{
    public partial class FileService
    {
        public FileService()
        {
            AppDataDirectory = Path.Combine(FileSystem.Current.AppDataDirectory, "Data");
        }
        partial void PrepareDatabaseFile()
        {
            // Checks if the file already exists.
            var dbPath = Path.Combine(AppDataDirectory, DatabaseName);

            if (File.Exists(dbPath))
            {
                return;
            }

            Task.WaitAll(CopyDatabaseFile());
        }

        async Task CopyDatabaseFile()
        {
            using (FileStream writeStream = new FileStream(Path.Combine(AppDataDirectory, DatabaseName), FileMode.Create, FileAccess.Write))
            {
                // Gets the realm database file from assets 
                using var dbFileStream = await FileSystem.OpenAppPackageFileAsync(DatabaseName);
                // Copies to the device
                dbFileStream.CopyTo(writeStream);
            }
        }
    }
}