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
        [Parameter] public Action UpvoteHandler { get; set; }
        [Parameter] public Action DownvoteHandler { get; set; }

        public bool CanRemove()
        {
            return UserStore.IsLoggedIn && UserStore.CurrentUser!.CanRemove(Entry.Rate, Entry.UserId, Entry.CreatedAt);
        }
    }
}
