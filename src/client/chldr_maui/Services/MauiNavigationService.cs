using chldr_domain.Interfaces;

namespace dosham.Services
{
    internal class MauiNavigationService : INavigationService
    {
        public async Task GoToAsync(string page)
        {
            await Shell.Current.GoToAsync($"//{page}");
        }
    }
}