namespace Validot
{
    using System.Linq;

    internal static class CodeHelper
    {
        public static bool IsCodeValid(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return false;
            }

            return code.All(c => !char.IsWhiteSpace(c));
        }
    }
}
