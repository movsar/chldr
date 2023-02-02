using chldr_shared.Validators;
using chldr_shared.Dto;

namespace chldr_ui.ViewModels
{
    public class RegistrationPageViewModel : EditFormViewModelBase<UserInfoDto, UserInfoValidator>
    {
        public UserInfoDto UserInfo { get; } = new();
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
