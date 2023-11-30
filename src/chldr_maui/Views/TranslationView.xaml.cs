using dosham.ViewModels;

namespace dosham.Views;

public partial class TranslationView : ContentView
{
	public TranslationView()
	{
		InitializeComponent();
        BindingContext = new TranslationViewModel();
    }
}