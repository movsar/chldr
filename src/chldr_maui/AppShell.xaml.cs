using dosham.Stores;
using ReactiveUI.Fody.Helpers;
using System.ComponentModel;

namespace dosham
{
    public partial class AppShell : Shell, INotifyPropertyChanged
    {
        private readonly UserStore _store;
        private string? _userEmail;
        public string? UserEmail
        {
            get
            {
                return _userEmail;
            }
            set
            {
                _userEmail = value;
                OnPropertyChanged(nameof(UserEmail));
            }
        }
        public bool IsLoggedIn => _store != null && _store.IsLoggedIn;
        public AppShell(UserStore store)
        {
            InitializeComponent();
            BindingContext = this;

            _store = store;
            _store.UserStateHasChanged += Store_UserStateHasChanged;
        }

        private void Store_UserStateHasChanged()
        {
            var user = _store.CurrentUser;
            if (user != null && (user.Status == chldr_data.Enums.UserStatus.Active || user.Status == chldr_data.Enums.UserStatus.Unconfirmed))
            {
                mnuProfile.FlyoutItemIsVisible = true;
                mnuLogin.FlyoutItemIsVisible = false;
                UserEmail = _store?.CurrentUser?.Email;
            }
        }
    }
}
