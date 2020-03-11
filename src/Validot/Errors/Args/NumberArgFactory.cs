namespace Validot
{
    using Validot.Errors.Args;

    public static partial class Arg
    {
        public static IArg Number(string name, int value)
        {
            return new NumberArg<int>(name, value, (v, format, cultureInfo) => v.ToString(format, cultureInfo));
        }

        public static IArg Number(string name, uint value)
        {
            return new NumberArg<uint>(name, value, (v, format, cultureInfo) => v.ToString(format, cultureInfo));
        }

        public static IArg Number(string name, float value)
        {
            return new NumberArg<float>(name, value, (v, format, cultureInfo) => v.ToString(format, cultureInfo));
        }

        public static IArg Number(string name, double value)
        {
            return new NumberArg<double>(name, value, (v, format, cultureInfo) => v.ToString(format, cultureInfo));
        }

        public static IArg Number(string name, decimal value)
        {
            return new NumberArg<decimal>(name, value, (v, format, cultureInfo) => v.ToString(format, cultureInfo));
        }

        public static IArg Number(string name, byte value)
        {
            return new NumberArg<byte>(name, value, (v, format, cultureInfo) => v.ToString(format, cultureInfo));
        }

        public static IArg Number(string name, sbyte value)
        {
            return new NumberArg<sbyte>(name, value, (v, format, cultureInfo) => v.ToString(format, cultureInfo));
        }

        public static IArg Number(string name, long value)
        {
            return new NumberArg<long>(name, value, (v, format, cultureInfo) => v.ToString(format, cultureInfo));
        }

        public static IArg Number(string name, ulong value)
        {
            return new NumberArg<ulong>(name, value, (v, format, cultureInfo) => v.ToString(format, cultureInfo));
        }

        public static IArg Number(string name, short value)
        {
            return new NumberArg<short>(name, value, (v, format, cultureInfo) => v.ToString(format, cultureInfo));
        }

        public static IArg Number(string name, ushort value)
        {
            return new NumberArg<ushort>(name, value, (v, format, cultureInfo) => v.ToString(format, cultureInfo));
        }
    }
}
