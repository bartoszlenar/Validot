namespace Validot;

using Validot.Errors.Args;

public static partial class Arg
{
    public static IArg Enum<T>(string name, T value)
        where T : struct
    {
        return new EnumArg<T>(name, value);
    }
}
