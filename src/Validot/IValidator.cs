namespace Validot
{
    using Validot.Results;
    using Validot.Settings;

    public interface IValidator<T>
    {
        ValidatorSettings Settings { get; }

        IValidationResult ErrorMap { get; }

        bool IsValid(T model);

        IValidationResult Validate(T model, bool failFast = false);
    }
}
