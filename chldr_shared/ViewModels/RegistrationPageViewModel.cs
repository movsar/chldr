using chldr_shared.Validators;
using chldr_shared.Dto;

namespace chldr_shared.ViewModels
{
    public class RegistrationPageViewModel : EditFormViewModel<UserInfoDto, UserInfoValidator>
    {
        public UserInfoDto UserInfo { get; } = new();
        private async Task SendRegistrationRequest()
        {
            await ContentStore.RegisterNewUser(UserInfo.Email, UserInfo.Password);
        }

        public override async Task ValidateAndSubmit()
        {
            await ValidateAndSubmit(UserInfo, new string[] { "Email", "Name", "Password" }, SendRegistrationRequest);
        }
    }
}
