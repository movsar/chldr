using chldr_data.Enums;
using chldr_data.Interfaces;
using chldr_utils.Services;
using System.IO.Compression;

namespace dosham
{
    public partial class App : Application
    {
        public static IServiceProvider Services { get; private set; }
        public App(IServiceProvider serviceProvider)
        {
            InitializeComponent();

            Services = serviceProvider;

            var fileService = Services.GetRequiredService<FileService>();
            var environmentService = Services.GetRequiredService<IEnvironmentService>();

            DeployInitialData(fileService, environmentService).ConfigureAwait(false);
            MainPage = new AppShell();
        }

        public static async Task DeployInitialData(FileService fileService, IEnvironmentService environmentService)
        {
            var dataZipPath = fileService.DataArchiveFilePath;
            var dataDirPath = fileService.AppDataDirectory;

            if (File.Exists(fileService.DatabaseFilePath))
            {
                return;
            }

            if (environmentService.CurrentPlatform != Platforms.Windows)
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
                    throw new Exception($"Error during file operation: {ex.Message}");
                }
            }

            try
            {
                ZipFile.ExtractToDirectory(dataZipPath, dataDirPath);                
            }
            catch (Exception ex)
            {
                throw new Exception($"Error during unpacking: {ex.Message}");
            }
        }


    }
}
