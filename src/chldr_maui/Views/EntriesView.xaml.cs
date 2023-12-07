using chldr_data.DatabaseObjects.Models;
using dosham.ViewModels;
using ReactiveUI;
using ReactiveUI.Maui;

namespace dosham.Views;

public partial class EntriesView : ReactiveContentView<EntriesViewModel>
{
    public static readonly BindableProperty EntriesProperty =
        BindableProperty.Create(nameof(Entries), typeof(IEnumerable<EntryModel>), typeof(EntriesView));
    public IEnumerable<EntryModel> Entries
    {
        get => (IEnumerable<EntryModel>)GetValue(EntriesProperty);
        set => SetValue(EntriesProperty, value);
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();
    }

    public EntriesView()
    {
        ViewModel = App.Services.GetRequiredService<EntriesViewModel>();

        this.WhenActivated(disposables =>
        {
            this.OneWayBind(ViewModel, vm => vm.Entries, v => v.Entries);
        });

        InitializeComponent();
    }
}