namespace Validot.Validation
{
    using Validot.Errors;
    using Validot.Validation.Scopes;

    internal interface IDiscoveryContextActions
    {
        IDiscoverable GetDiscoverableSpecificationScope(int id);

        int RegisterError(IError error);
    }
}
