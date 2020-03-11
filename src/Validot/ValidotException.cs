namespace Validot
{
    using System;

    public class ValidotException : Exception
    {
        public ValidotException(string message)
            : base(message)
        {
        }

        public ValidotException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
