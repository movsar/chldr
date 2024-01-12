using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reactive;

namespace dosham.ViewModels
{
    public class LoginPageViewModel : EditFormViewModelBase
    {
        [Reactive] public string Email { get; set; }
        [Reactive] public string Password { get; set; }
        public ReactiveCommand<Unit, Unit> LoginCommand { get; }
        public ReactiveCommand<Unit, Unit> RegisterCommand { get; }

        public LoginPageViewModel()
        {
            LoginCommand = ReactiveCommand.CreateFromTask(OnLogin);
            RegisterCommand = ReactiveCommand.CreateFromTask(OnRegister);
        }

        private async Task OnRegister()
        {
            await Shell.Current.GoToAsync("//Register");
        }

        private async Task OnLogin()
        {
            await ExecuteSafelyAsync(() => UserStore.LogInEmailPasswordAsync(Email!, Password!));
        }

    }
}
