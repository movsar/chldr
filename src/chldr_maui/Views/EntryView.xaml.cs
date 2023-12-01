using chldr_data.DatabaseObjects.Models;
using dosham.ViewModels;

namespace dosham.Views;

public partial class EntryView : ContentView
{
    public static readonly BindableProperty EntryProperty =
         BindableProperty.Create(nameof(Entry), typeof(EntryModel), typeof(EntryView));

    public EntryModel Entry
    {
        get => (EntryModel)GetValue(EntryProperty);
        set => SetValue(EntryProperty, value);
    }

    public EntryView()
	{
        var viewModel = App.Services.GetRequiredService<EntryViewModel>();
        viewModel.Entry = Entry;
        BindingContext = viewModel;
		InitializeComponent();
    }
}