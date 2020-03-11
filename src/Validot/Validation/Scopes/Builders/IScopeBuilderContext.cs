namespace Validot.Validation.Scopes.Builders
{
    using Validot.Errors;

    internal interface IScopeBuilderContext
    {
        int DefaultErrorId { get; }

        int ForbiddenErrorId { get; }

        int RequiredErrorId { get; }

        int RegisterError(IError error);

        int GetOrRegisterSpecificationScope<T>(Specification<T> specification);
    }
}
