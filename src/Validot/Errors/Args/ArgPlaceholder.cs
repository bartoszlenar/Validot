namespace Validot.Errors.Args
{
    using System.Collections.Generic;

    public sealed class ArgPlaceholder
    {
        public string Placeholder { get; set; }

        public string Name { get; set; }

        public IReadOnlyDictionary<string, string> Parameters { get; set; }
    }
}
