using core.DatabaseObjects.Models;
using chldr_application.ViewModels;
using ReactiveUI;
using ReactiveUI.Maui;

namespace dosham.Views;
public partial class EntriesView : ReactiveContentView<EntriesViewModel>
{
    public EntriesView()
    {
        ViewModel = App.Services.GetRequiredService<EntriesViewModel>();
        InitializeComponent();
    }
}