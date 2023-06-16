
namespace chldr_ui.Interfaces
{
    public interface IEditFormViewModel<TFormDto, TFormValidator>
    {
        bool FormSubmitted { get; set; }
        Task ValidateAndSubmitAsync(TFormDto formDto, Func<Task> function, string[]? validationRuleSets = null);
    }
}
