using chldr_application.ViewModels;
using ReactiveUI;
using ReactiveUI.Maui;

namespace dosham.Pages;

public partial class AlphabetPage : ReactiveContentPage<AlphabetPageViewModel>
{
    public AlphabetPage()
    {
        ViewModel = App.Services.GetRequiredService<AlphabetPageViewModel>();

        InitializeComponent();

        this.WhenActivated(disposable =>
        {
            foreach (var child in slButtons.Children)
            {
                if (child is ImageButton imageButton)
                {
                    imageButton.Command = ViewModel.ImageButtonClicked;
                    imageButton.CommandParameter = imageButton.Source;
                }
            }
        });
    }
}