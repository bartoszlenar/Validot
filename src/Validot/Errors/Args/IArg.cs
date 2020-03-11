namespace Validot.Errors.Args
{
    using System.Collections.Generic;

    public interface IArg
    {
        string Name { get; }

        IReadOnlyCollection<string> AllowedParameters { get; }

        string ToString(IReadOnlyDictionary<string, string> parameters);
    }

    public interface IArg<out T> : IArg
    {
        T Value { get; }
    }
}
