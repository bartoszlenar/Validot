namespace Validot.Errors
{
    public sealed class CacheIntegrityException : ValidotException
    {
        public CacheIntegrityException(string message)
            : base(message)
        {
        }
    }
}
