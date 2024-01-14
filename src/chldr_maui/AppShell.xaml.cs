using dosham.Stores;
using ReactiveUI.Fody.Helpers;

namespace dosham
{
    public partial class AppShell : Shell
    {
        private readonly UserStore _store;
        public string? UserEmail => _store?.CurrentUser?.Email;
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
            }
        }
    }
}
