using System.IO.Compression;

namespace chldr_android
{
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : Activity
    {
        public async Task ExtractZipFile(Activity activity)
        {
            string appFilesPath = Application.Context.FilesDir.AbsolutePath;
            
            if (File.Exists($"{appFilesPath}/database/local.datx"))
            {
                return;
            }

            try
            {
                // Get the ZIP file as a stream from the raw resources
                using (Stream zipStream = activity.Assets.Open("data.zip"))
                {
                    // Ensure the directory exists
                    Directory.CreateDirectory(appFilesPath);

                    // Use ZipArchive to extract the ZIP file
                    using (ZipArchive archive = new ZipArchive(zipStream))
                    {
                        foreach (ZipArchiveEntry entry in archive.Entries)
                        {
                            string fullPath = Path.Combine(appFilesPath, entry.FullName);

                            // Check if the entry is a directory
                            if (String.IsNullOrEmpty(entry.Name))
                            {
                                Directory.CreateDirectory(fullPath);
                            }
                            else
                            {
                                // Ensure the directory for the file exists
                                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

                                // Extract the file asynchronously
                                using (var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
                                {
                                    await entry.Open().CopyToAsync(fileStream);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                Console.WriteLine("Error extracting ZIP file: " + ex.Message);
            }
        }
        protected override  void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

        }

        protected override async void OnResume()
        {
            base.OnResume();
            await ExtractZipFile(this);
        }
    }
}