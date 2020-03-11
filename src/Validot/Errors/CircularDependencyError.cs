namespace Validot.Errors
{
    using System;
    using System.Collections.Generic;

    using Validot.Errors.Args;
    using Validot.Translations;

    internal sealed class CircularDependencyError : IError
    {
        public CircularDependencyError(Type type)
        {
            Messages = new[]
            {
                MessageKey.Global.CircularDependency
            };

            Args = new[]
            {
                Arg.Type("type", type)
            };

            Codes = Array.Empty<string>();
        }

        public IReadOnlyList<string> Messages { get; }

        public IReadOnlyList<string> Codes { get; }

        public IReadOnlyList<IArg> Args { get; }
    }
}
