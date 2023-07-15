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

            builder.RegisterNativeAppServices();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }

    }
}