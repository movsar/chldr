using dosham.Stores;
using dosham.ViewModels;
using ReactiveUI;
using ReactiveUI.Maui;
using System.Reactive.Disposables;
using Microsoft.Extensions.DependencyInjection;

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

        public MainPage()
        {
            InitializeComponent();
            this.WhenActivated(SetupBindings);
        }

        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();
            var serviceProvider = Handler.MauiContext.Services;
            ViewModel = serviceProvider.GetRequiredService<MainPageViewModel>();
        }

        private void SetupBindings(CompositeDisposable disposables)
        {
            this.OneWayBind(ViewModel, vm => vm.FilteredEntries, v => v.EntriesListView.ItemsSource)
                .DisposeWith(disposables);

            this.Bind(ViewModel, vm => vm.SearchText, v => v.SearchBox.Text)
               .DisposeWith(disposables);
        }
    }
}
