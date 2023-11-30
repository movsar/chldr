namespace dosham.Pages;

public partial class IndexPage : ContentPage
{
    public IndexPage()
    {
        InitializeComponent();
        BindingContext = App.Services.GetRequiredService<IndexPageViewModel>();
    }
}