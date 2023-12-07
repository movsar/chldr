using dosham.ViewModels;
using ReactiveUI.Maui;

namespace dosham.Pages;
public partial class MainPage : ReactiveContentPage<MainPageViewModel>
{
    public MainPage()
    {
        InitializeComponent();
        BindingContext = App.Services.GetRequiredService<MainPageViewModel>();
    }
}