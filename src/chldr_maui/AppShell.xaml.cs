using chldr_app.Stores;
using chldr_data.Resources.Localizations;
using Microsoft.Extensions.Localization;
using System.ComponentModel;

namespace dosham
{
    public partial class AppShell : Shell, INotifyPropertyChanged
    {
        #region Properties, Fields and Events
        private readonly IStringLocalizer<AppLocalizations> _localizer;
        private readonly UserStore _store;
        private string? _header;
        public string? Header
        {
            get => _header;
            set
            {
                _header = value;
                OnPropertyChanged(nameof(Header));
            }
        }
        private bool _isLoggedIn;
        public bool IsLoggedIn
        {
            get => _isLoggedIn;
            set
            {
                _isLoggedIn = value;
                OnPropertyChanged(nameof(IsLoggedIn));
            }
        }
        #endregion

        public AppShell(UserStore store, IStringLocalizer<AppLocalizations> stringLocalizer)
        {
            InitializeComponent();
            BindingContext = this;

            _store = store;
            _localizer = stringLocalizer;

            _store.UserStateHasChanged += Store_UserStateHasChanged;
            _ = _store.RestoreLastSession();
        }

        private void Store_UserStateHasChanged()
        {
            var user = _store.CurrentUser;
            if (user != null && (user.Status == chldr_data.Enums.UserStatus.Active || user.Status == chldr_data.Enums.UserStatus.Unconfirmed))
            {
                Header = string.Format(_localizer["Hello{0}!"], _store?.CurrentUser?.GetFullName());
                
                mnuLogin.FlyoutItemIsVisible = false;
                mnuProfile.FlyoutItemIsVisible = true;
                IsLoggedIn = true;
            }
            else
            {
                mnuProfile.FlyoutItemIsVisible = false;
                mnuLogin.FlyoutItemIsVisible = true;
                mnuLogin.IsVisible = true;
                IsLoggedIn = false;
            }
        }
    }
}
