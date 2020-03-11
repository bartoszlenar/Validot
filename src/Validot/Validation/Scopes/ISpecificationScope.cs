namespace Validot.Validation.Scopes
{
    using System.Collections.Generic;

    internal interface ISpecificationScope<T> : IValidatable<T>, IDiscoverable
    {
        IReadOnlyList<ICommandScope<T>> CommandScopes { get; set; }

        Presence Presence { get; set; }

        int ForbiddenErrorId { get; set; }

        int RequiredErrorId { get; set; }
    }
}
