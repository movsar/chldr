using dosham.ViewModels;
using ReactiveUI;
using ReactiveUI.Maui;

namespace dosham.Pages;
public partial class MainPage : ReactiveContentPage<MainPageViewModel>
{
    public MainPage()
    {
        ViewModel = App.Services.GetRequiredService<MainPageViewModel>();
        InitializeComponent();

        this.WhenActivated(disposable =>
        {
            this.OneWayBind(ViewModel, vm => vm.FilteredEntries, v => v.entriesView.ViewModel!.Entries);
        });
    }
}