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
        [Parameter] public Action PromoteHandler { get; set; }
    }
}
