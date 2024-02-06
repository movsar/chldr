using core.DatabaseObjects.Dtos;
using chldr_shared.Validators;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reactive;
using chldr_app.Stores;

namespace chldr_application.ViewModels
{
    public class RegistrationPageViewModel : EditFormViewModelBase<UserDto, UserInfoValidator>
    {
        private readonly UserStore _userStore;

        [Reactive] public UserDto UserInfo { get; set; } = new();
        public ReactiveCommand<Unit, Unit> RegisterCommand { get; }

        public RegistrationPageViewModel(UserStore userStore, UserInfoValidator userValidator)
        {
            _userStore = userStore;
            DtoValidator = userValidator;

            RegisterCommand = ReactiveCommand.CreateFromTask(OnRegister);
        }

        private async Task OnRegister()
        {
            await ValidateAndSubmitAsync(UserInfo, async () =>
            {
                await _userStore.RegisterNewUser(UserInfo.Email!, UserInfo.Password!);
            }, new string[] { "Email", "Name", "Password" });
        }
    }
}
