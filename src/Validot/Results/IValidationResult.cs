namespace Validot.Results
{
    public interface IValidationResult
    {
        bool AnyErrors { get; }

        IValidationResultDetails Details { get; }
    }
}
