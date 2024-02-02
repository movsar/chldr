using core.DatabaseObjects.Dtos;
using chldr_shared.Validators;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reactive;

namespace dosham.ViewModels
{
    public class LoginPageViewModel : EditFormViewModelBase<UserDto, UserInfoValidator>
    {
        [Reactive] public UserDto UserInfo { get; set; } = new();
        public static bool EmailConfirmationCompleted { get; private set; } = false;
        public ReactiveCommand<Unit, Unit> LoginCommand { get; }
        public ReactiveCommand<Unit, Unit> RegisterCommand { get; }

        public LoginPageViewModel()
        {
            LoginCommand = ReactiveCommand.CreateFromTask(OnLogin);
            RegisterCommand = ReactiveCommand.CreateFromTask(OnRegister);
        }
        private async Task SignInWithEmailPassword()
        {
            await UserStore.LogInEmailPasswordAsync(UserInfo.Email!, UserInfo.Password!);
            await GoToAsync("Search");
        }

        private async Task OnRegister()
        {
            await GoToAsync("Register");
        }

        public async Task OnLogin()
        {
            await ValidateAndSubmitAsync(UserInfo, SignInWithEmailPassword, new string[] {
                nameof(UserInfo.Email), nameof(UserInfo.Password)
            });
        }
    }
}
