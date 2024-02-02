using chldr_app.Stores;
using chldr_data.Enums;
using chldr_data.Interfaces;
using chldr_data.Resources.Localizations;
using Microsoft.Extensions.Localization;
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

            var fileService = Services.GetRequiredService<IFileService>();
            var environmentService = Services.GetRequiredService<IEnvironmentService>();
            var stringLocalizer = Services.GetRequiredService<IStringLocalizer<AppLocalizations>>();
            var userStore = Services.GetRequiredService<UserStore>();

            DeployInitialData(fileService, environmentService).ConfigureAwait(false);
            MainPage = new AppShell(userStore, stringLocalizer);
        }

        public static async Task DeployInitialData(IFileService fileService, IEnvironmentService environmentService)
        {
            var dataZipPath = fileService.DataArchiveFilePath;
            var dataDirPath = fileService.AppDataDirectory;

            if (File.Exists(fileService.DatabaseFilePath))
            {
                return;
            }

            if (environmentService.CurrentPlatform != Platforms.Windows)
            {
                var packagedDatafileExists = await FileSystem.Current.AppPackageFileExistsAsync(fileService.DataArchiveName);
                if (!packagedDatafileExists)
                {
                    throw new Exception("Data file does not exist in the package");
                }

                try
                {
                    using (var dataStream = await FileSystem.Current.OpenAppPackageFileAsync(fileService.DataArchiveName))
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
