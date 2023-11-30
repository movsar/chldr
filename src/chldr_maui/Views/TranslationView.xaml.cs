using dosham.ViewModels;

namespace dosham.Views;

public partial class TranslationView : ContentView
{
	public TranslationView()
	{
		InitializeComponent();
        BindingContext = App.Services.GetRequiredService<TranslationViewModel>();
    }
}