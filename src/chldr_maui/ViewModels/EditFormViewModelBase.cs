using ReactiveUI.Fody.Helpers;

namespace dosham.ViewModels
{
    public abstract class EditFormViewModelBase: ViewModelBase
    {
        [Reactive] public string ErrorMessage { get; set; }
        protected bool ExecuteSafely(Action action)
        {
            ErrorMessage = "";

            try
            {
                action();
                return true;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return false;
            }
        }
        protected async Task<bool> ExecuteSafelyAsync(Func<Task> func)
        {
            ErrorMessage = "";

            try
            {
                await func();
                return true;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return false;
            }
        }
    }
}
