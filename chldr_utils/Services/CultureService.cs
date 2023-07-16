namespace chldr_utils.Services
{
    public class CultureService
    {
        private string currentCulture;

        public event Action<string>? CurrentCultureChanged;

        public string CurrentCulture
        {
            get
            {
                return currentCulture;
            }
            set
            {
                if (currentCulture != value)
                {
                    currentCulture = value;
                    CurrentCultureChanged?.Invoke(currentCulture);
                }
            }
        }
    }
}
