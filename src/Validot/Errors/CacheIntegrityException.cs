namespace Validot.Errors
{
    using System;

    public class CacheIntegrityException : Exception
    {
        public CacheIntegrityException(string message)
            : base(message)
        {
        }
    }
}
