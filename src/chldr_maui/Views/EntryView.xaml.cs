using dosham.ViewModels;

namespace dosham.Views;

public partial class EntryView : ContentView
{
	public EntryView()
	{
		InitializeComponent();
        BindingContext = App.Services.GetRequiredService<EntryViewModel>();
    }
}