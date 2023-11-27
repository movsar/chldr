namespace dosham.Pages;

public partial class IndexPage : ContentPage
{
    public IndexPage()
    {
        InitializeComponent();
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();

        var serviceProvider = Handler!.MauiContext!.Services;
        BindingContext = serviceProvider.GetRequiredService<IndexPageViewModel>();
    }
}