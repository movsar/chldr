using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Enums;
using Microsoft.AspNetCore.Components;

namespace chldr_ui.ViewModels
{
    public class TranslationActionButtonsViewModel : ViewModelBase
    {
        [Parameter] public TranslationDto TranslationDto { get; set; }
        [Parameter] public Action RemoveHandler { get; set; }
        [Parameter] public Action EditHandler { get; set; }

        public bool CanEdit()
        {
            // Anyone should be able to open an entry for edit mode, if they're logged in and active
            // However, they might not be able to change anything, that will be governed by CanEdit* methods
            return UserStore.IsLoggedIn && UserStore.CurrentUser!.CanEdit(TranslationDto.Rate, TranslationDto.UserId);
        }

        public bool CanRemove()
        {
            return UserStore.IsLoggedIn && UserStore.CurrentUser!.CanRemove(TranslationDto.Rate, TranslationDto.UserId, TranslationDto.CreatedAt);
        }
    }
}
