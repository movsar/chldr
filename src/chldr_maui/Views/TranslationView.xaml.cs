using chldr_data.DatabaseObjects.Models;
using dosham.ViewModels;

namespace dosham.Views;

public partial class TranslationView : ContentView
{
    public static readonly BindableProperty TranslationProperty =
     BindableProperty.Create(nameof(Translation), typeof(TranslationModel), typeof(TranslationView));

    public TranslationModel Translation
    {
        get => (TranslationModel)GetValue(TranslationProperty);
        set => SetValue(TranslationProperty, value);
    }

    public TranslationView()
	{
        var viewModel = App.Services.GetRequiredService<TranslationViewModel>();
        viewModel.Translation = Translation;
        BindingContext = viewModel;
		InitializeComponent();
    }
}