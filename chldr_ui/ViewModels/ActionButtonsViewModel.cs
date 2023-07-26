using chldr_data.DatabaseObjects.Models;
using Microsoft.AspNetCore.Components;

namespace chldr_ui.ViewModels
{
    public class ActionButtonsViewModel : ViewModelBase
    {
        [Parameter] public EntryModel Entry { get; set; }
        [Parameter] public Action ShareHandler { get; set; }
        [Parameter] public Action RemoveHandler { get; set; }

        public bool CanEdit()
        {
            return IsLoggedInUser && UserStore.CurrentUser!.CanEditEntry(Entry!.Rate!);
        }

        public bool CanRemove()
        {
            return IsLoggedInUser && UserStore.CurrentUser!.CanRemoveEntry(Entry);
        }

        public bool IsLoggedInUser => UserStore.IsLoggedIn;
    }
}
