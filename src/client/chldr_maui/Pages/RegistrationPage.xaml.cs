using chldr_application.ViewModels;
using ReactiveUI.Maui;

namespace dosham.Pages;

public partial class RegistrationPage : ReactiveContentPage<RegistrationPageViewModel>
{
    public RegistrationPage()
    {
        ViewModel = App.Services.GetRequiredService<RegistrationPageViewModel>();        
        InitializeComponent();
    }
}