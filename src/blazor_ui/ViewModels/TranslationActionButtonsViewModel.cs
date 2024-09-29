using domain.DatabaseObjects.Dtos;
using Microsoft.AspNetCore.Components;

namespace chldr_ui.ViewModels
{
    public class TranslationActionButtonsViewModel : ViewModelBase
    {
        [Parameter] public TranslationDto TranslationDto { get; set; }
        [Parameter] public Action RemoveHandler { get; set; }
        [Parameter] public Action EditHandler { get; set; }
        [Parameter] public Action PromoteHandler { get; set; }
    }
}
