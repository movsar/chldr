using Microsoft.AspNetCore.Components;

namespace chldr_ui.ViewModels
{
    public class PagerViewModel : ViewModelBase
    {
        [Parameter] public int CurrentPage { get; set; }
        [Parameter] public int TotalPages { get; set; }
        [Parameter] public Action<int> SelectedPage { get; set; }

        internal void OnPreviousPage()
        {
            if (CurrentPage > 1)
            {
                CurrentPage -= 1;
                SelectedPage?.Invoke(CurrentPage);
            }
        }

        internal void OnNextPage()
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage += 1;
                SelectedPage?.Invoke(CurrentPage);
            }
        }
    }
}
