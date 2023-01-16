using chldr_blazor.Stores;
using Data.Services;
using Microsoft.Extensions.DependencyInjection;

namespace chldr_blazor
{
    public partial class App : Application
    {
        public static ContentStore ContentStore;
        public static IServiceProvider ServiceProvider;
        public App(IServiceProvider serviceProvider, ContentStore contentStore)
        {
            InitializeComponent();

            MainPage = new MainPage();

            ServiceProvider = serviceProvider;
            ContentStore = contentStore;
            
            // TODO: Clean up the database
            // 1. column renamings - done
            // 2. change grammar class to int - done
            // 3. change byte to int - done
            // 4. check other stuff - done
            // 5. remove weitd things Ψ - done
            // 6. setup device sync for realm - done
            // 7. setup user management
            // 8. remove notes from Entry to Text/Phrase/Word
        }
    }
}