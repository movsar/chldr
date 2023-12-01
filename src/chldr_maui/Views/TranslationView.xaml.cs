using chldr_data.DatabaseObjects.Models;
using dosham.ViewModels;

namespace dosham.Views;

public partial class TranslationView : ContentView
{
    private readonly TranslationViewModel _viewModel;

    public static readonly BindableProperty TranslationProperty =
     BindableProperty.Create(nameof(_viewModel.Translation), typeof(TranslationModel), typeof(TranslationView));

    public TranslationView()
	{
        _viewModel = App.Services.GetRequiredService<TranslationViewModel>();
        BindingContext = _viewModel;

		InitializeComponent();
    }
}