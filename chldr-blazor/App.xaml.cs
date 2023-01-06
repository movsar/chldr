using Microsoft.Extensions.DependencyInjection;

namespace chldr_blazor
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider;
        public App(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            MainPage = new MainPage();
            ServiceProvider = serviceProvider;
        }
    }
}