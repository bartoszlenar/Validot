namespace Validot.Validation
{
    using System.Collections.Generic;

    public interface IErrorsHolder
    {
        Dictionary<string, List<int>> Errors { get; }
    }
}
