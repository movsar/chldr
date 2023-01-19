namespace chldr_native
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void blazorWebView_BlazorWebViewInitialized(object sender, Microsoft.AspNetCore.Components.WebView.BlazorWebViewInitializedEventArgs e)
        {
#if WINDOWS
            e.WebView.CoreWebView2.Settings.IsZoomControlEnabled = false;
#endif
        }
    }
}