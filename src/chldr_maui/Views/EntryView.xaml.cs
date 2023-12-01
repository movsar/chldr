using chldr_data.DatabaseObjects.Models;
using dosham.ViewModels;

namespace dosham.Views;

public partial class EntryView : ContentView
{
    private EntryViewModel _viewModel;

    public static readonly BindableProperty EntryProperty =
         BindableProperty.Create(nameof(_viewModel.Entry), typeof(EntryModel), typeof(EntryView));

    public EntryView()
	{
        _viewModel = App.Services.GetRequiredService<EntryViewModel>();
        BindingContext = _viewModel;
		InitializeComponent();
    }
}