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

        public override async Task ValidateAndSubmit()
        {
            await ValidateAndSubmit(UserInfo, new string[] { "Email", "Name", "Password" }, SendRegistrationRequest);
        }
    }
}
