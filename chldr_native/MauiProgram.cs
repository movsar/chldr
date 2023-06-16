using Blazored.Modal;
using chldr_native.Extensions;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace chldr_native
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder.UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

            builder.Services.AddLocalization();
            builder.Services.AddBlazoredModal();

            builder.RegisterNativeAppServices();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            var culture = CultureInfo.GetCultureInfo("ru_RU");

            Thread.CurrentThread.CurrentUICulture = culture;
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;

            return builder.Build();
        }

    }
}