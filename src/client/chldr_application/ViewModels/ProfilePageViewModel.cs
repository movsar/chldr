using chldr_app.Stores;
using ReactiveUI;
using System.Reactive;

namespace chldr_application.ViewModels
{
    public class ProfilePageViewModel : ViewModelBase
    {
        private readonly UserStore _userStore;

        public ReactiveCommand<Unit, Unit> LogoutCommand { get; }

        public ProfilePageViewModel(UserStore userStore)
        {
            _userStore = userStore;
            LogoutCommand = ReactiveCommand.CreateFromTask(OnLogout);
        }

        public async Task OnLogout()
        {
            await _userStore.LogOutAsync();
            await GoToAsync("Search");
        }
    }
}
