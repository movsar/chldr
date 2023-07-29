using chldr_data.DatabaseObjects.Models;
using chldr_data.Enums;
using Microsoft.AspNetCore.Components;

namespace chldr_ui.ViewModels
{
    public class EntryActionButtonsViewModel : ViewModelBase
    {
        [Parameter] public EntryModel Entry { get; set; }
        [Parameter] public Action ShareHandler { get; set; }
        [Parameter] public Action RemoveHandler { get; set; }

        public bool CanEdit()
        {
            // Anyone should be able to open an entry for edit mode, if they're logged in and active
            // However, they might not be able to change anything, that will be governed by CanEdit* methods
            return UserStore.IsLoggedIn && UserStore.CurrentUser!.Status == UserStatus.Active;
        }

        public bool CanRemove()
        {
            return UserStore.IsLoggedIn && UserStore.CurrentUser!.CanRemove(Entry.Rate, Entry.UserId, Entry.CreatedAt);
        }
    }
}
