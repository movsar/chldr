using core.DatabaseObjects.Dtos;
using chldr_shared.Validators;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reactive;

namespace chldr_application.ViewModels
{
    public class RegistrationPageViewModel : EditFormViewModelBase<UserDto, UserInfoValidator>
    {
        [Reactive] public UserDto UserInfo { get; set; } = new();
        public ReactiveCommand<Unit, Unit> RegisterCommand { get; }

        public RegistrationPageViewModel()
        {
            RegisterCommand = ReactiveCommand.CreateFromTask(OnRegister);
        }

        private async Task OnRegister()
        {
            await ValidateAndSubmitAsync(UserInfo, async () =>
            {
                await UserStore.RegisterNewUser(UserInfo.Email!, UserInfo.Password!);
            }, new string[] { "Email", "Name", "Password" });
        }
    }
}
