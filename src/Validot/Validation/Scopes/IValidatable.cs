namespace Validot.Validation.Scopes
{
    internal interface IValidatable<in T>
    {
        void Validate(T model, IValidationContext context);
    }
}
