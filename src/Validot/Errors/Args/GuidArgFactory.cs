namespace Validot
{
    using System;

    using Validot.Errors.Args;

    public static partial class Arg
    {
        public static IArg GuidValue(string name, Guid value)
        {
            return new GuidArg(name, value);
        }
    }
}
