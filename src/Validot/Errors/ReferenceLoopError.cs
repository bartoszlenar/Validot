namespace Validot.Errors
{
    using System;
    using System.Collections.Generic;

    using Validot.Errors.Args;
    using Validot.Translations;

    internal class ReferenceLoopError : IError
    {
        public ReferenceLoopError(Type type)
        {
            Messages = new[]
            {
                MessageKey.Global.ReferenceLoop
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
