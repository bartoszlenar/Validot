namespace Validot.Errors
{
    public class CacheIntegrityException : ValidotException
    {
        public CacheIntegrityException(string message)
            : base(message)
        {
        }
    }
}
