using core.DatabaseObjects.Models;
using dosham.ViewModels;
using ReactiveUI;
using System.Reactive.Disposables;

namespace dosham.Views;

public abstract class AbstractEntryView<TViewModel> : ContentView, IViewFor<TViewModel> where TViewModel : class
{
    protected readonly CompositeDisposable SubscriptionDisposables = new CompositeDisposable();

    public static readonly BindableProperty ViewModelProperty =
        BindableProperty.Create(nameof(ViewModel),
            typeof(TViewModel),
            typeof(IViewFor<TViewModel>),
            null,
            BindingMode.OneWay,
            null,
            new BindableProperty.BindingPropertyChangedDelegate(OnViewModelChanged),
            null,
            null,
            null);

    /// <summary>The ViewModel to display</summary>
    public TViewModel ViewModel
    {
        get => (TViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();
        ViewModel = BindingContext as TViewModel;
    }

    private static void OnViewModelChanged(BindableObject bindableObject, object oldValue, object newValue)
    {
        bindableObject.BindingContext = newValue;
    }

    object IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (TViewModel)value;
    }

    
}