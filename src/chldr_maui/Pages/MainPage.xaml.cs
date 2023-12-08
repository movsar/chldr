using dosham.ViewModels;
using ReactiveUI;
using ReactiveUI.Maui;
using System.Reactive.Disposables;

namespace dosham.Pages;
public partial class MainPage : ReactiveContentPage<MainPageViewModel>
{
    public MainPage()
    {
        ViewModel = App.Services.GetRequiredService<MainPageViewModel>();
        InitializeComponent();

        // Reactive bindings
        this.WhenActivated(disposable =>
        {
            this.OneWayBind(ViewModel, vm => vm.FilteredEntries, v => v.entriesView.ViewModel!.Entries);
        });
    }
}