namespace Validot;

using Validot.Errors.Args;

public static partial class Arg
{
    public static IArg Type(string name, Type value)
    {
        return new TypeArg(name, value);
    }
}
