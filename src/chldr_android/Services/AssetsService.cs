using System.IO.Compression;

namespace chldr_android.Services
{
    internal class AssetsService
    {
        public static async Task ExtractInitialDataFromAssets(Activity activity, string zipName)
        {
            if (Application.Context.FilesDir == null)
            {
                throw new Exception("Files dir is null");
            }

            string appDataPath = $"{Application.Context.FilesDir.AbsolutePath}/data";
            if (File.Exists($"{appDataPath}/database/local.datx"))
            {
                return;
            }

            if (activity.Assets == null)
            {
                throw new Exception("Assets was null");
            }

            try
            {
                // Get the ZIP file as a stream from the raw resources
                using (Stream zipStream = activity.Assets.Open(zipName))
                using (ZipArchive archive = new ZipArchive(zipStream))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        string entryPath = Path.Combine(appDataPath, entry.FullName);
                        if (entry.FullName.EndsWith("/"))
                        {
                            Directory.CreateDirectory(entryPath);
                        }
                        else
                        {
                            // Ensure the file's directory exists
                            Directory.CreateDirectory(Path.GetDirectoryName(entryPath)!);

                            // Extract the file
                            entry.ExtractToFile(entryPath, overwrite: true);
                        }
                    }
                }

                // Give it some time to unpack and rest :)
                await Task.Delay(1000);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while extracting the data file", ex);
            }
        }
    }
}
