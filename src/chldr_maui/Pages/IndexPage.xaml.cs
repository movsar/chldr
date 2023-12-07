using dosham.ViewModels;
using ReactiveUI.Maui;

namespace dosham.Pages;

public partial class IndexPage : ReactiveContentPage<IndexPageViewModel>
{
    public IndexPage()
    {
        InitializeComponent();
        // App.Services.GetRequiredService<IndexPageViewModel>();
    }
}