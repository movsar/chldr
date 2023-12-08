using dosham.ViewModels;
using ReactiveUI;
using ReactiveUI.Maui;

namespace dosham.Pages;

public partial class IndexPage : ReactiveContentPage<IndexPageViewModel>
{
    public IndexPage()
    {
        ViewModel = App.Services.GetRequiredService<IndexPageViewModel>();

        InitializeComponent();

        this.WhenActivated(disposable =>
        {
            this.OneWayBind(ViewModel, vm => vm.Entries, v => v.entriesView.ViewModel!.Entries);
            this.BindCommand(ViewModel, vm => vm.BtnNextClickedCommand, v => v.BtnNext);
            this.BindCommand(ViewModel, vm => vm.BtnPreviousClickedCommand, v => v.BtnPrevious);
        });
    }
}