using core.DatabaseObjects.Dtos;
using chldr_shared.Validators;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reactive;
using chldr_app.Stores;
using chldr_domain.Interfaces;

namespace chldr_application.ViewModels
{
    public class RegistrationPageViewModel : EditFormViewModelBase<UserDto, UserInfoValidator>
    {
        private readonly UserStore _userStore;
        private readonly INavigationService _navigationService;

        [Reactive] public UserDto UserInfo { get; set; } = new();
        public ReactiveCommand<Unit, Unit> RegisterCommand { get; }

        public RegistrationPageViewModel(
            UserStore userStore, 
            UserInfoValidator userValidator,
            INavigationService navigationService)
        {
            _userStore = userStore;
            _navigationService = navigationService;

            DtoValidator = userValidator;

            RegisterCommand = ReactiveCommand.CreateFromTask(OnRegister);
        }

        private async Task OnRegister()
        {
            await ValidateAndSubmitAsync(UserInfo, async () =>
            {
                await _userStore.RegisterNewUser(UserInfo.Email!, UserInfo.Password!);
                await _navigationService.GoToAsync("Search");
            }, new string[] { "Email", "Name", "Password" });
        }
    }
}
