using dosham.ViewModels;
using ReactiveUI.Maui;

namespace dosham.Pages;

public partial class ProfilePage : ReactiveContentPage<ProfilePageViewModel>
{
	public ProfilePage()
	{
		ViewModel = App.Services.GetRequiredService<ProfilePageViewModel>();
		InitializeComponent();
	}
}