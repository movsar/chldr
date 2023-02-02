
namespace chldr_ui.Interfaces
{
    public interface IEditFormViewModel<TFormDto, TFormValidator>
    {
        bool FormDisabled { get; set; }
        bool FormSubmitted { get; set; }
        void ValidateAndSubmit(TFormDto formDto, Action action, string[]? validationRuleSets = null);
        Task ValidateAndSubmitAsync(TFormDto formDto, Func<Task> function, string[]? validationRuleSets = null);
    }
}
