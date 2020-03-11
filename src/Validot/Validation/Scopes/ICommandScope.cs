namespace Validot.Validation.Scopes
{
    using System;

    using Validot.Validation.Scopes.Builders;

    internal interface ICommandScope
    {
    }

    internal interface ICommandScope<T> : ICommandScope, IScope<T>
    {
        string Name { get; set; }

        int? ErrorId { get; set; }

        ErrorMode ErrorMode { get; set; }

        Predicate<T> ShouldExecute { get; set; }
    }
}
