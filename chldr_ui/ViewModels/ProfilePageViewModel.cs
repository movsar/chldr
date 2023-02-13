using chldr_data.Dto;
using chldr_shared.Validators;
using Microsoft.AspNetCore.Components;

namespace chldr_ui.ViewModels
{
    public class ProfilePageViewModel : EditFormViewModelBase<UserInfoDto, UserInfoValidator>
    {
        [Inject] NavigationManager NavigationManager { get; set; }
        public Task ValidateAndSubmitAsync()
        {
            throw new NotImplementedException();
        }

        public async Task LogOutAsync()
        {
            UserStore.LogOutAsync();
            NavigationManager.NavigateTo("/");
        }
    }
}
