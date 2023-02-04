
/* Unmerged change from project 'chldr_native (net6.0-windows10.0.19041.0)'
Before:
using chldr_shared.Stores;
using chldr_data.Services;
After:
using chldr_data.Services;
using chldr_shared.Stores;
*/

/* Unmerged change from project 'chldr_native (net6.0-maccatalyst)'
Before:
using chldr_shared.Stores;
using chldr_data.Services;
After:
using chldr_data.Services;
using chldr_shared.Stores;
*/

/* Unmerged change from project 'chldr_native (net6.0-android)'
Before:
using chldr_shared.Stores;
using chldr_data.Services;
After:
using chldr_data.Services;
using chldr_shared.Stores;
*/
namespace chldr_native
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();

            // TODO: Clean up the database
            // 1. column renamings - done
            // 2. change grammar class to int - done
            // 3. change byte to int - done
            // 4. check other stuff - done
            // 5. remove weitd things Ψ - done
            // 6. setup device sync for realm - done
            // 7. fix phrase notes - done
            // 8. word / phrase editing
            // 9. setup user management
        }
    }
}