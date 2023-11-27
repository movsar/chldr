namespace dosham
{
    public partial class App : Application
    {
        protected override async void OnStart()
        {
            base.OnStart();

            // Initialize the database
            await DatabaseInitializer.InitializeAsync();
        }
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }
    }
}
