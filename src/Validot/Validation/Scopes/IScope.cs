namespace Validot.Validation.Scopes
{
    internal interface IScope<in T> : IValidatable<T>, IDiscoverable
    {
    }
}
