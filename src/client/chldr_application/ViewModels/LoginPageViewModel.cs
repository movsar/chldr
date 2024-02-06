using core.DatabaseObjects.Dtos;
using chldr_shared.Validators;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reactive;
using chldr_app.Stores;
using chldr_domain.Interfaces;

namespace chldr_application.ViewModels
{
    public class LoginPageViewModel : EditFormViewModelBase<UserDto, UserInfoValidator>
    {
        [Reactive] public UserDto UserInfo { get; set; } = new();
        public static bool EmailConfirmationCompleted { get; private set; } = false;

        private readonly UserStore _userStore;
        private readonly INavigationService _navigationService;

        public ReactiveCommand<Unit, Unit> LoginCommand { get; }
        public ReactiveCommand<Unit, Unit> RegisterCommand { get; }

        public LoginPageViewModel(UserStore userStore, UserInfoValidator userValidator, INavigationService navigationService)
        {
            DtoValidator = userValidator;
            _userStore = userStore;
            _navigationService = navigationService;

            LoginCommand = ReactiveCommand.CreateFromTask(OnLogin);
            RegisterCommand = ReactiveCommand.CreateFromTask(OnRegister);
        }
        private async Task SignInWithEmailPassword()
        {
            await _userStore.LogInEmailPasswordAsync(UserInfo.Email!, UserInfo.Password!);
            await _navigationService.GoToAsync("Search");
        }

        private async Task OnRegister()
        {
            await _navigationService.GoToAsync("Register");
        }

        public async Task OnLogin()
        {
            await ValidateAndSubmitAsync(UserInfo, SignInWithEmailPassword, new string[] {
                nameof(UserInfo.Email), nameof(UserInfo.Password)
            });
        }
    }
}
