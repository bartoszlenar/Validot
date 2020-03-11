namespace Validot.Results
{
    public interface IValidationResult
    {
        bool IsValid { get; }

        IValidationResultDetails Details { get; }
    }
}
