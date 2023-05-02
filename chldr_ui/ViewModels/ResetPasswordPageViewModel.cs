using chldr_data.Dto;
using chldr_shared.Validators;

namespace chldr_ui.ViewModels
{
    public class ResetPasswordPageViewModel : EditFormViewModelBase<UserDto, UserInfoValidator>
    {
        public UserDto UserInfo { get; set; } = new();
        private async Task SendPasswordResetRequest()
        {
            await UserStore.SendPasswordResetRequestAsync(UserInfo.Email);
        }
        public async Task ValidateAndSubmitAsync()
        {
            await ValidateAndSubmitAsync(UserInfo, SendPasswordResetRequest, new string[] { "Email" });
        }
    }
}
