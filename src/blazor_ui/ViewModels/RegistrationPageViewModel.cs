using domain.DatabaseObjects.Dtos;
using chldr_shared.Validators;

namespace chldr_ui.ViewModels
{
    public class RegistrationPageViewModel : EditFormViewModelBase<UserDto, UserInfoValidator>
    {
        public UserDto UserInfo { get; } = new();
        private async Task SendRegistrationRequest()
        {
            await UserStore.RegisterNewUser(UserInfo.Email, UserInfo.Password);
        }

        public async Task ValidateAndSubmitAsync()
        {
            await ValidateAndSubmitAsync(UserInfo, SendRegistrationRequest, new string[] { "Email", "Name", "Password" });
        }
    }
}
