using ReactiveUI;
using System.Reactive;

namespace chldr_application.ViewModels
{
    public class ProfilePageViewModel : ViewModelBase
    {
        public ReactiveCommand<Unit, Unit> LogoutCommand { get; }

        public ProfilePageViewModel()
        {
            LogoutCommand = ReactiveCommand.CreateFromTask(OnLogout);
        }

        public async Task OnLogout()
        {
            await UserStore.LogOutAsync();
            await GoToAsync("Search");
        }
    }
}
