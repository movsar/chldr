namespace dosham.Pages;
public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        BindingContext = App.Services.GetRequiredService<MainPageViewModel>();
    }
}