namespace Validot
{
    using Validot.Results;
    using Validot.Settings;

    public interface IValidator<T>
    {
        IValidatorSettings Settings { get; }

        IValidationResult ErrorMap { get; }

        bool IsValid(T model);

        IValidationResult Validate(T model, bool failFast = false);
    }
}
