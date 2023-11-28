namespace dosham.Pages;
public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }        

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();

        var serviceProvider = Handler!.MauiContext!.Services;
        BindingContext = serviceProvider.GetRequiredService<MainPageViewModel>();
    }
}