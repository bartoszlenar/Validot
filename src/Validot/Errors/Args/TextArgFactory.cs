namespace Validot
{
    using Validot.Errors.Args;

    public static partial class Arg
    {
        public static IArg Text(string name, string value)
        {
            return new TextArg(name, value);
        }

        public static IArg Text(string name, char value)
        {
            return new TextArg(name, value);
        }
    }
}
