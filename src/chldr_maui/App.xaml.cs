using chldr_data.Interfaces;
using chldr_utils.Services;
using dosham.Services;
using dosham.Stores;
using System.Diagnostics;
using System.IO.Compression;

namespace dosham
{
    public partial class App : Application
    {
        public IServiceProvider Services { get; }
        public App(IServiceProvider serviceProvider)
        {
            InitializeComponent();

            Services = serviceProvider;

            var fileService = Services.GetRequiredService<FileService>();
            var environmentService = Services.GetRequiredService<IEnvironmentService>();

            if (DeployInitialData(fileService, environmentService).Result == true)
            {
                MainPage = new AppShell();
            }
        }

        public static async Task<bool> DeployInitialData(FileService fileService, IEnvironmentService environmentService)
        {
            var dataZipPath = fileService.DataArchiveFilePath;
            var dataDirPath = fileService.AppDataDirectory;

            if (File.Exists(fileService.DatabaseFilePath))
            {
                return true;
            }

            if (environmentService.CurrentPlatform != Enums.Platforms.Windows)
            {
                var packagedDatafileExists = await FileSystem.Current.AppPackageFileExistsAsync(FileService.DataArchiveName);
                if (!packagedDatafileExists)
                {
                    throw new Exception("Data file does not exist in the package");
                }

                try
                {
                    using (var dataStream = await FileSystem.Current.OpenAppPackageFileAsync(FileService.DataArchiveName))
                    {
                        using (var fileStream = File.Create(dataZipPath))
                        {
                            await dataStream.CopyToAsync(fileStream).ConfigureAwait(false);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error during file operation: {ex.Message}");
                    return false; // Return false on exception
                }
            }

            try
            {
                Debug.WriteLine("Unpacking...");
                ZipFile.ExtractToDirectory(dataZipPath, dataDirPath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error during unpacking: {ex.Message}");
                return false; // Return false on exception
            }

            return true;
        }


    }
}
