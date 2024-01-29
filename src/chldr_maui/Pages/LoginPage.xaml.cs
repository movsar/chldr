using dosham.ViewModels;
using ReactiveUI.Maui;

namespace dosham.Pages;

public partial class LoginPage : ReactiveContentPage<LoginPageViewModel>
{
    public LoginPage()
    {
        ViewModel = App.Services.GetRequiredService<LoginPageViewModel>();        
        InitializeComponent();
    }
}