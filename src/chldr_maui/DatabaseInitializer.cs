namespace dosham
{
    internal class DatabaseInitializer
    {
        private const string DatabaseFilename = "local.datx";

        public static async Task InitializeAsync()
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);

            if (!File.Exists(dbPath))
            {
                var dbAssetPath = $"{DatabaseFilename}";
                using (var dbAssetStream = await FileSystem.OpenAppPackageFileAsync(dbAssetPath))
                {
                    using (var fileStream = File.Create(dbPath))
                    {
                        await dbAssetStream.CopyToAsync(fileStream);
                    }
                }
            }

        }
    }
}