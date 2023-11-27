using dosham.Stores;
using dosham.ViewModels;
using ReactiveUI;
using ReactiveUI.Maui;
using System.Reactive.Disposables;

namespace dosham
{
    public partial class MainPage : ContentPage, IViewFor<MainPageViewModel>
    {
        public static readonly BindableProperty ViewModelProperty = BindableProperty.Create(
            nameof(ViewModel), typeof(MainPageViewModel), typeof(MainPage), null, BindingMode.OneWay);

        public MainPageViewModel ViewModel
        {
            get => (MainPageViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (MainPageViewModel)value;
        }
        private ContentStore _contentStore;
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
            ViewModel = new MainPageViewModel();

            this.WhenActivated(disposables =>
            {
                this.BindCommand(ViewModel, vm => vm.CounterCommand, v => v.CounterBtn)
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel, vm => vm.Count, v => v.CounterBtn.Text,
                                count => $"Clicked {count} " + (count == 1 ? "time" : "times"))
                    .DisposeWith(disposables);
            });
        }

        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();
            _contentStore = Handler.MauiContext.Services.GetRequiredService<ContentStore>();
        }


    }

}
