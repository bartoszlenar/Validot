namespace Validot.Errors;

public sealed class CacheIntegrityException(string message) : ValidotException(message)
{
}
