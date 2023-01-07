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

            // TODO: 1. Clean up the database
            // 1. column renamings
            // 2. change grammar class to int
            // 3. change byte to int
            // 4. check other stuff
            // 5. setup device sync for realm
            // 6. setup user management
        }
    }
}