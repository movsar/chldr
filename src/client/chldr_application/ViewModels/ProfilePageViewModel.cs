using chldr_app.Stores;
using chldr_domain.Interfaces;
using ReactiveUI;
using System.Reactive;

namespace chldr_application.ViewModels
{
    public class ProfilePageViewModel : ViewModelBase
    {
        private readonly UserStore _userStore;
        private readonly INavigationService _navigationService;

        public ReactiveCommand<Unit, Unit> LogoutCommand { get; }

        public ProfilePageViewModel(UserStore userStore, INavigationService navigationService)
        {
            _userStore = userStore;
            _navigationService = navigationService;
            LogoutCommand = ReactiveCommand.CreateFromTask(OnLogout);
        }

        public async Task OnLogout()
        {
            await _userStore.LogOutAsync();
            await _navigationService.GoToAsync("Search");
        }
    }
}
